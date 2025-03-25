using Microsoft.Extensions.AI;

namespace OllamaChatClientExample.Server.Chat;

public interface IChatHistoryService
{
    Task<List<ChatMessage>> GetChatHistory(Guid chatId, CancellationToken ct);

    Task<List<ChatMessage>> AddToChatHistory(Guid chatId, ChatRole role, string message, CancellationToken ct);

    Task Clear(Guid id, CancellationToken ct);
}
