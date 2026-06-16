using PaperGames.Client.Models;

namespace PaperGames.Client.Services;

public class AppState
{
    public string MyPeerId { get; set; } = "";
    public string MyNickname { get; set; } = "";
    public bool IsHost { get; set; }
    public string LobbyCode { get; set; } = "";
    public List<Player> Players { get; set; } = new();
    public GameState? CurrentGame { get; set; }

    public event Action? OnChange;

    public void NotifyStateChanged() => OnChange?.Invoke();

    public void Reset()
    {
        MyPeerId = "";
        MyNickname = "";
        IsHost = false;
        LobbyCode = "";
        Players = new();
        CurrentGame = null;
    }
}

public class GameState
{
    public string OpponentId { get; set; } = "";
    public string OpponentNickname { get; set; } = "";
    public bool IAmBlack { get; set; }
    public bool IsMyTurn { get; set; }
    public Dictionary<(int X, int Y), bool> Stones { get; set; } = new();  // true = black stone
    public bool IsOver { get; set; }
    public string? WinnerId { get; set; }
    public bool RematchRequested { get; set; }
    public bool OpponentRequestedRematch { get; set; }
}
