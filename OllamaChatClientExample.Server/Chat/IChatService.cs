namespace OllamaChatClientExample.Server.Chat;

public interface IChatService
{
    Task<Guid> NewChat(string message);
    Task<string> ContinueChat(Guid id, string message);
    Task<string> GetReply(Guid id);
    Task<IEnumerable<string>> GetConversation(Guid id);
    Task Clear(Guid id);
}