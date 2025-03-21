using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text;

namespace OllamaChatClientExample.Server.Chat;

public class ChatService : IChatService
{
    private const string THINKING_ABOUT_IT = "Still thinking about it...";

    private IChatClient _chatClient;
    private HybridCache _cache;

    public ChatService(IChatClient chatClient, HybridCache cache)
    {
        _chatClient = chatClient;
        _cache = cache;
    }

    public async Task<Guid> NewChat(string message)
    {
        // Generate a unique id (or use supplied id) so we can track the conversation
        var chatId = Guid.NewGuid();

        // Start a new chat conversation
        var chatHistory = await AddToChatHistory(chatId, ChatRole.User, message);

        _ = Task.Run(async () => {
            // Perform the long-running chat operation
            var chatCompletionResponse = await _chatClient.GetResponseAsync(chatHistory);

            var chatResponseBuilder = new StringBuilder();
            foreach (var messageResponse in chatCompletionResponse.Messages)
            {
                chatResponseBuilder.AppendLine(messageResponse.Text);
            }

            var chatResponse = chatResponseBuilder.ToString();
            await AddToChatHistory(chatId, ChatRole.Assistant, chatResponse);
        });

        return chatId;
    }

    public async Task<string> AddToChat(Guid id, string message)
    {
        // Check if the chat is awaiting a response (last entry will be the user message)
        var lastChatHistoryEntry = await GetLastChatHistoryEntry(id);

        if (lastChatHistoryEntry is null || lastChatHistoryEntry.Role == ChatRole.User)
        {
            return THINKING_ABOUT_IT;
        }

        var updatedChatHistory = await AddToChatHistory(id, ChatRole.User, message);

        _ = Task.Run(async () => {
            // Perform the long-running chat operation
            var chatCompletionResponse = await _chatClient.GetResponseAsync(updatedChatHistory);

            var chatResponseBuilder = new StringBuilder();
            foreach (var messageResponse in chatCompletionResponse.Messages)
            {
                chatResponseBuilder.AppendLine(messageResponse.Text);
            }

            var chatResponse = chatResponseBuilder.ToString();
            await AddToChatHistory(id, ChatRole.Assistant, chatResponse);
        });

        return lastChatHistoryEntry.Text;
    }

    public async Task<string> GetReply(Guid id)
    {
        // Check if the chat is awaiting a response (last entry will be the user message)
        var lastChatHistoryEntry = await GetLastChatHistoryEntry(id);

        if (lastChatHistoryEntry is null || lastChatHistoryEntry.Role == ChatRole.User)
        {
            return THINKING_ABOUT_IT;
        }

        return lastChatHistoryEntry.Text;
    }

    /// <summary>
    /// Add a message to the chat history, if the message is User, then append a system waiting message, if the message is Assistant, remove the system waiting message+
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="role"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task<List<ChatMessage>> AddToChatHistory(Guid chatId, ChatRole role, string message)
    {
        var chatTag = BuildChatTag(chatId);
        var chatHistoryCacheKey = BuildChatHistoryCacheKey(chatId);
        var chatMessage = new ChatMessage(role, message);
        var chatHistory = await _cache.GetOrCreateAsync(chatHistoryCacheKey, async _ => await Task.FromResult(new List<ChatMessage>()), tags: [chatTag]);
        chatHistory.Add(chatMessage);
        await _cache.SetAsync(chatHistoryCacheKey, chatHistory, tags: [chatTag]);
        return chatHistory;
    }

    /// <summary>
    /// Add a message to the chat history, if the message is User, then append a system waiting message, if the message is Assistant, remove the system waiting message+
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="role"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task<ChatMessage?> GetLastChatHistoryEntry(Guid chatId)
    {
        var chatTag = BuildChatTag(chatId);
        var chatHistoryCacheKey = BuildChatHistoryCacheKey(chatId);
        var chatHistory = await _cache.GetOrCreateAsync(chatHistoryCacheKey, async _ => await Task.FromResult(new List<ChatMessage>()), tags: [chatTag]);
        return chatHistory.LastOrDefault();
    }

    public async Task Clear(Guid id) => await _cache.RemoveByTagAsync(BuildChatTag(id));

    private static string BuildChatTag(Guid id) => $"ChatService.Chat({id})";
    private static string BuildChatHistoryCacheKey(Guid id) => $"ChatService.ChatHistory({id})";
}
