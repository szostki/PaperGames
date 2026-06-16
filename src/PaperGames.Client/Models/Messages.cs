using System.Text.Json;

namespace PaperGames.Client.Models;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
}

public record Player(string Id, string Nickname);

public record JoinMessage(string Type, string Nickname);
public record PlayerListMessage(string Type, List<Player> Players);
public record InviteMessage(string Type, string FromId, string ToId);
public record InviteAcceptedMessage(string Type);
public record MoveMessage(string Type, int X, int Y);
public record MoveAckMessage(string Type, int X, int Y, string PlayerId, bool IsBlack);
public record GameOverMessage(string Type, string WinnerId);
public record RematchRequestMessage(string Type);
public record RematchAcceptedMessage(string Type);
public record RematchDeclinedMessage(string Type);
