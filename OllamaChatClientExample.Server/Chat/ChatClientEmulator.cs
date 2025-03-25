using Microsoft.Extensions.AI;

namespace OllamaChatClientExample.Server.Chat;

public class ChatClientEmulator : IChatClient
{
    private int _responseTime;

    public ChatClientEmulator(int responseTime)
    {
        _responseTime = responseTime;
    }

    public void Dispose()
    {
        // Do nothing
    }


    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return null;
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    async Task<Microsoft.Extensions.AI.ChatResponse> IChatClient.GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options, CancellationToken cancellationToken)
    {
        var lastMessage = messages.LastOrDefault() ?? new ChatMessage(ChatRole.System, "No messages");

        await Task.Delay(_responseTime);
        return await Task.FromResult(new Microsoft.Extensions.AI.ChatResponse
        {
            Messages = new[]
            {
                new ChatMessage(ChatRole.Assistant, $"{DateTime.UtcNow:HH:mm:ss.ff} - I've thought about '{lastMessage.Text}'")
            },
            CreatedAt = DateTime.UtcNow
        });
    }
}
