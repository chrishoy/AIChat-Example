namespace OllamaChatClientExample.Server.Chat;

public interface IChatService
{
    Task<Guid> NewChat(string message);
    Task<string> AddToChat(Guid id, string message);
    Task<string> GetReply(Guid id);
    Task Clear(Guid id);
}