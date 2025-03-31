namespace OllamaChatClientExample.Server.Chat;

public interface IChatService
{
    Task<Message> Chat(string message, Guid? id, CancellationToken ct = default);
    Task<Message> GetLastResponse(Guid id, CancellationToken ct = default);
    Task<Conversation> GetConversation(Guid id, CancellationToken ct = default);
}