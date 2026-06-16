using Microsoft.JSInterop;
using PaperGames.Client.Models;
using System.Text.Json;

namespace PaperGames.Client.Services;

public class PeerJSService : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private DotNetObjectReference<PeerJSService>? _dotnetRef;

    public event Action<string, string>? MessageReceived;
    public event Action<string>? PeerConnected;
    public event Action<string>? PeerDisconnected;
    public event Action<string>? PeerError;

    public PeerJSService(IJSRuntime js) => _js = js;

    public async Task InitHostAsync(string lobbyCode)
    {
        EnsureRef();
        await _js.InvokeVoidAsync("PeerJSInterop.initHost", lobbyCode, _dotnetRef);
    }

    public async Task<string> InitGuestAsync()
    {
        EnsureRef();
        return await _js.InvokeAsync<string>("PeerJSInterop.initGuest", _dotnetRef);
    }

    public Task ConnectToHostAsync(string hostId) =>
        _js.InvokeVoidAsync("PeerJSInterop.connectTo", hostId).AsTask();

    public Task SendAsync(string targetPeerId, object message) =>
        _js.InvokeVoidAsync("PeerJSInterop.sendTo", targetPeerId,
            JsonSerializer.Serialize(message, JsonOptions.Default)).AsTask();

    public Task BroadcastAsync(object message) =>
        _js.InvokeVoidAsync("PeerJSInterop.sendToAll",
            JsonSerializer.Serialize(message, JsonOptions.Default)).AsTask();

    public Task DestroyAsync() =>
        _js.InvokeVoidAsync("PeerJSInterop.destroy").AsTask();

    [JSInvokable]
    public void OnMessageReceived(string fromPeerId, string json) =>
        MessageReceived?.Invoke(fromPeerId, json);

    [JSInvokable]
    public void OnPeerConnected(string peerId) =>
        PeerConnected?.Invoke(peerId);

    [JSInvokable]
    public void OnPeerDisconnected(string peerId) =>
        PeerDisconnected?.Invoke(peerId);

    [JSInvokable]
    public void OnPeerError(string message) =>
        PeerError?.Invoke(message);

    private void EnsureRef()
    {
        _dotnetRef?.Dispose();
        _dotnetRef = DotNetObjectReference.Create(this);
    }

    public async ValueTask DisposeAsync()
    {
        try { await DestroyAsync(); } catch { }
        _dotnetRef?.Dispose();
    }
}
