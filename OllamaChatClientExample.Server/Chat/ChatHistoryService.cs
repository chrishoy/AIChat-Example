using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Hybrid;

namespace OllamaChatClientExample.Server.Chat;

public class ChatHistoryService: IChatHistoryService
{
    private readonly HybridCache _cache;

    public ChatHistoryService(HybridCache cache)
    {
        _cache = cache;
    }

    public async Task<List<ChatMessage>> AddToChatHistory(Guid chatId, ChatRole role, string message, CancellationToken ct)
    {
        var chatTag = ChatHelpers.BuildChatTag(chatId);
        var chatHistoryCacheKey = ChatHelpers.BuildChatHistoryCacheKey(chatId);
        var chatMessage = new ChatMessage(role, message);

        var props = new AdditionalPropertiesDictionary();
        props.Add("Timestamp", DateTimeOffset.UtcNow);

        chatMessage.AdditionalProperties = props;
        var chatHistory = await _cache.GetOrCreateAsync(chatHistoryCacheKey, async _ => await Task.FromResult(new List<ChatMessage>()), tags: [chatTag], cancellationToken: ct);

        chatHistory.Add(chatMessage);
        await _cache.SetAsync(chatHistoryCacheKey, chatHistory, tags: [chatTag], cancellationToken: ct);

        return chatHistory;
    }

    public async Task<List<ChatMessage>> GetChatHistory(Guid chatId, CancellationToken ct)
    {
        var chatTag = ChatHelpers.BuildChatTag(chatId);
        var chatHistoryCacheKey = ChatHelpers.BuildChatHistoryCacheKey(chatId);
        var chatHistory = await _cache.GetOrCreateAsync(chatHistoryCacheKey, async _ => await Task.FromResult(new List<ChatMessage>()), tags: [chatTag], cancellationToken: ct);

        return chatHistory;
    }

    public async Task Clear(Guid id, CancellationToken ct) => 
        await _cache.RemoveByTagAsync(ChatHelpers.BuildChatTag(id), cancellationToken: ct);
}
