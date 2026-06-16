# PaperGames

Browser-based LAN party games — no install, no signup. Host a lobby on your Wi‑Fi, invite friends, and play.

**Live app:** [https://szostki.github.io/PaperGames/](https://szostki.github.io/PaperGames/)

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (for local development only)
- Modern browser with WebRTC support (Chrome, Edge, Firefox, Safari)
- Players must be reachable over the network — same Wi‑Fi works reliably; strict corporate/guest networks may block WebRTC

## How to play

### Hosting

1. Open the app, enter a nickname, click **Host lobby**.
2. The lobby shows a QR code and a shareable join link.

> **Keep this tab open** for the whole session — closing it ends the lobby.

### Joining

**Via QR code or link (easiest):**
3. On another device, scan the QR code or open the join link — the lobby code is pre-filled automatically.
4. Enter a nickname and click **Join lobby**.

**Manually:**
3. Ask the host for their 6-character lobby code.
4. Open the app, enter the code in the **Join** field, enter a nickname, click **Join lobby**.

### In the lobby

Once connected, the lobby shows all players. The host clicks **Invite** next to any player to start a game.

### Winning

First to get **5 in a row** (horizontal, vertical, or diagonal) on an infinite grid wins.

### Playing again

After a game ends, either player can click **Play again** to request a rematch. The opponent sees the request and can **accept** or **decline**. On accept, a new game starts instantly — the loser of the previous round gets to go first (plays black). No need to return to the lobby.

## Local development

```bash
dotnet run --project src/PaperGames.Client/PaperGames.Client.csproj
```

Open `http://localhost:5215/` in your browser.

To test the full connection flow, open **two browser windows** side by side: host in one, join in the other using the lobby code shown on the host screen.

> The `wasm-tools` .NET workload is required. Install it once with:
> ```bash
> dotnet workload install wasm-tools
> ```

## Architecture

- **Blazor WebAssembly** (.NET 10) — compiled to WASM, runs entirely in the browser, hosted as static files on GitHub Pages
- **PeerJS** (WebRTC DataChannels) — peer-to-peer messaging via the public PeerJS cloud signaling server; no backend server required
- **Host-as-hub** — the host's tab is authoritative: it validates moves, maintains lobby state, and broadcasts updates to all peers
- **AppState singleton** — shared in-memory state (identity, lobby players, current game) passed between pages without a router outlet
- **Message protocol** — typed camelCase JSON records with a `type` discriminator (`join`, `player_list`, `invite`, `invite_accepted`, `move`, `move_ack`, `game_over`, `rematch_request`, `rematch_accepted`, `rematch_declined`)

Closing the host tab ends the entire session. There is no server-side persistence.

## Deployment

Deployed automatically to GitHub Pages on every push to `main` via `.github/workflows/deploy-pages.yml`:

1. Checks out the repo and sets up .NET 10 + `wasm-tools` workload
2. Publishes the Blazor WASM project in Release mode
3. Patches `<base href="/" />` → `<base href="/PaperGames/" />` in the published `index.html` (required for GitHub Pages subpath hosting)
4. Uploads `publish/wwwroot` and deploys via `actions/deploy-pages`

## Game: Infinite 5-in-a-row (Gomoku)

- Two players (black stones and white stones), alternating turns
- Pan the board by **dragging** (mouse or one finger), zoom with the **scroll wheel** or **pinch**
- **Click or tap** an intersection to place your stone
- The grid is  limited 120x120 — only cells that have been played are stored
- Win detection checks all four directions (horizontal, vertical, two diagonals) from the placed stone
- Works on **PC, tablet, and phone** — touch pan, pinch-zoom, and tap-to-place are all supported

## License

MIT
