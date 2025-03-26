using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Hybrid;
using System.Threading.Channels;

namespace OllamaChatClientExample.Server.Chat;

public class ChatService : IChatService
{
    private const string THINKING_ABOUT = "Thinking about ";
    private const string STILL_THINKING_ABOUT = "Please wait... Still thinking about ";

    private Channel<ChatChannelRequest> _chatChannel;
    private readonly IChatHistoryService _chatHistoryService;

    public ChatService(IChatClient chatClient, HybridCache cache, Channel<ChatChannelRequest> chatChannel, IChatHistoryService chatHistoryService)
    {
        _chatChannel = chatChannel;
        _chatHistoryService = chatHistoryService;
    }

    public async Task<ChatMessageSummary> Chat(string message, Guid? id, CancellationToken ct = default)
    {
        // Generate a unique id (or use supplied id) so we can track the conversation
        var chatId = id ?? Guid.NewGuid();

        // Start a new chat conversation
        var chatHistory = await _chatHistoryService.AddToChatHistory(chatId, ChatRole.User, message, ct);

        // Delegate request to ChatProcessor - Need to figure out how to determine if channel is full and can't accept more processing requests.
        await _chatChannel.Writer.WriteAsync(new ChatChannelRequest(chatId, chatHistory));

        return new ChatMessageSummary(chatId, $"{THINKING_ABOUT} {message}", ChatRole.Assistant, DateTimeOffset.UtcNow);
    }

    public async Task<string> ContinueChat(Guid id, string message, CancellationToken ct = default)
    {
        // Check if the chat is awaiting a response (last entry will be the user message)
        var thinkingText = CheckThinking((await _chatHistoryService.GetChatHistory(id, ct)).LastOrDefault());
        if (thinkingText is not null)
        {
            return thinkingText;
        }

        var updatedChatHistory = await _chatHistoryService.AddToChatHistory(id, ChatRole.User, message, ct);
        await _chatChannel.Writer.WriteAsync(new ChatChannelRequest(id, updatedChatHistory));
        return $"{THINKING_ABOUT}{message}";
    }

    public async Task<ChatMessageSummary> GetLastResponse(Guid id, CancellationToken ct = default)
    {
        var chatHistory = await _chatHistoryService.GetChatHistory(id, ct);
        if (!chatHistory.Any())
        {
            return new ChatMessageSummary(id, "No chat history", ChatRole.System, DateTimeOffset.UtcNow);
        }

        // Check if the chat is awaiting a response (last entry will be the user message)
        var lastChatHistoryEntry = chatHistory.Last();
        var thinkingText = CheckThinking(lastChatHistoryEntry);

        return thinkingText is not null
            ? new ChatMessageSummary(id, thinkingText, ChatRole.System, DateTimeOffset.UtcNow)
            : lastChatHistoryEntry.ToChatMessageSummary(id);
    }

    public async Task<IEnumerable<ChatMessageSummary>> GetConversation(Guid id, CancellationToken ct)
    {
        var chatHistory = await _chatHistoryService.GetChatHistory(id, ct);
        if (!chatHistory.Any())
        {
            return new List<ChatMessageSummary> { new ChatMessageSummary(id, "No chat history", ChatRole.System, DateTimeOffset.UtcNow) };
        }

        var conversation = chatHistory.ConvertAll(m => m.ToChatMessageSummary(id));
        string? thinkingText = CheckThinking(chatHistory.Last());
        if (thinkingText is not null)
        {
            return [.. conversation, new ChatMessageSummary(id, thinkingText, ChatRole.System, DateTimeOffset.UtcNow)];
        }

        return conversation;
    }

    #region Private Methods

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

    #endregion Private Methods
}
