namespace OllamaChatClientExample.Server.Chat;

public interface IChatService
{
    Task<Guid> NewChat(string message, CancellationToken ct);
    Task<string> GetReply(Guid id, CancellationToken ct);
    Task<string> ContinueChat(Guid id, string message, CancellationToken ct);
    Task<IEnumerable<string>> GetConversation(Guid id, CancellationToken ct);
}