using SoliGest.Server.Models;
using SoliGest.Server.Repositories;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

public static class ChatHandler
{
    private static readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    public static async Task HandleConnectionAsync(
        string userId, WebSocket socket, IServiceProvider services)
    {
        _sockets[userId] = socket;
        var repo = services.GetRequiredService<IChatRepository>();
        var buffer = new byte[4 * 1024];

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var incoming = JsonSerializer.Deserialize<ChatMessageDto>(json)!;

                    // Persist
                    var msgEntity = new ChatMessage
                    {
                        FromUserId = userId,
                        ToUserId = incoming.To,
                        Text = incoming.Text
                    };
                    await repo.SaveAsync(msgEntity);

                    // Prepare outgoing
                    var outgoing = new ChatMessageDto
                    {
                        From = userId,
                        Text = incoming.Text,
                        SentAt = msgEntity.SentAt
                    };
                    var payload = JsonSerializer.Serialize(outgoing);
                    var bytes = Encoding.UTF8.GetBytes(payload);

                    // Send to recipient
                    if (_sockets.TryGetValue(incoming.To, out var dest)
                        && dest.State == WebSocketState.Open)
                    {
                        await dest.SendAsync(
                            new ArraySegment<byte>(bytes),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }

                    // Echo back to sender
                    await socket.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if (result.MessageType == WebSocketMessageType.Close) break;
            }
        }
        finally
        {
            _sockets.TryRemove(userId, out _);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", CancellationToken.None);
        }
    }

    private record ChatMessageDto(string To = "", string From = "", string Text = "", DateTime SentAt = default);
}