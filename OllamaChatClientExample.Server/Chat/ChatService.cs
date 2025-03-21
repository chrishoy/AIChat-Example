using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text;

namespace OllamaChatClientExample.Server.Chat;

public class ChatService : IChatService
{
    private const string THINKING_ABOUT = "Thinking about ";
    private const string STILL_THINKING_ABOUT = "Please wait... Still thinking about ";

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
            // Perform the long-running chat operation (needs to be converted to background task)
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
        var amIStillThinking = CheckThinking(await GetLastChatHistoryEntry(id));
        if (amIStillThinking is not null)
        {
            return amIStillThinking;
        }

        var updatedChatHistory = await AddToChatHistory(id, ChatRole.User, message);

        _ = Task.Run(async () =>
        {
            // Perform the long-running chat operation (needs to be converted to background task)
            var chatCompletionResponse = await _chatClient.GetResponseAsync(updatedChatHistory);

            var chatResponseBuilder = new StringBuilder();
            foreach (var messageResponse in chatCompletionResponse.Messages)
            {
                chatResponseBuilder.AppendLine(messageResponse.Text);
            }

            var chatResponse = chatResponseBuilder.ToString();
            await AddToChatHistory(id, ChatRole.Assistant, chatResponse);
        });

        return $"{THINKING_ABOUT}{message}";
    }

    private static string? CheckThinking(ChatMessage? lastChatHistoryEntry)
    {
        if (lastChatHistoryEntry is null)
        {
            return "Chat not found";
        }
        else if (lastChatHistoryEntry.Role == ChatRole.User)
        {
            return $"{STILL_THINKING_ABOUT} {lastChatHistoryEntry.Text}";
        }

        return null;
    }

    public async Task<string> GetReply(Guid id)
    {
        // Check if the chat is awaiting a response (last entry will be the user message)
        var lastChatHistoryEntry = await GetLastChatHistoryEntry(id);
        var amIStillThinking = CheckThinking(lastChatHistoryEntry);
        if (amIStillThinking is not null)
        {
            return amIStillThinking;
        }

        return lastChatHistoryEntry?.Text ?? "Erm.. Something went wrong!";
    }

    public async Task Clear(Guid id) => await _cache.RemoveByTagAsync(BuildChatTag(id));

    #region Private Methods

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

    private async Task<ChatMessage?> GetLastChatHistoryEntry(Guid chatId)
    {
        var chatTag = BuildChatTag(chatId);
        var chatHistoryCacheKey = BuildChatHistoryCacheKey(chatId);
        var chatHistory = await _cache.GetOrCreateAsync(chatHistoryCacheKey, async _ => await Task.FromResult(new List<ChatMessage>()), tags: [chatTag]);

        return chatHistory.LastOrDefault();
    }

    private static string BuildChatTag(Guid id) => $"ChatService.Chat({id})";

    private static string BuildChatHistoryCacheKey(Guid id) => $"ChatService.ChatHistory({id})";

    #endregion Private Methods
}
