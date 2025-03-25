using Microsoft.Extensions.AI;

namespace OllamaChatClientExample.Server.Emulators;

public class ChatClientEmulator : IChatClient
{
    public void Dispose()
    {
        // Do nothing
    }

    public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        var lastMessage = messages.LastOrDefault() ?? new ChatMessage(ChatRole.System, "No messages");

        await Task.Delay(10000);
        return await Task.FromResult(new ChatResponse
        {
            Messages = new[]
            {
                new ChatMessage(ChatRole.Assistant, $"{DateTime.UtcNow:HH:mm:ss.ff} - I've thought about '{lastMessage.Text}'")
            },
            CreatedAt = DateTime.UtcNow
        });
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return null;
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
