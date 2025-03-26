namespace OllamaChatClientExample.Server.Chat;

public interface IChatService
{
    Task<ChatMessageSummary> Chat(string message, Guid? id, CancellationToken ct = default);
    Task<ChatMessageSummary> GetLastResponse(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ChatMessageSummary>> GetConversation(Guid id, CancellationToken ct = default);
}