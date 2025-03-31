using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Hybrid;
using System.Threading.Channels;

namespace OllamaChatClientExample.Server.Chat;

public class ChatService : IChatService
{
    private const string CHAT_NOT_FOUND = "Chat not found";
    private const string THINKING_ABOUT = "Thinking about ";
    private const string STILL_THINKING_ABOUT = "Please wait... Still thinking about ";

    private Channel<ChatChannelRequest> _chatChannel;
    private readonly IChatHistoryService _chatHistoryService;

    public ChatService(IChatClient chatClient, HybridCache cache, Channel<ChatChannelRequest> chatChannel, IChatHistoryService chatHistoryService)
    {
        _chatChannel = chatChannel;
        _chatHistoryService = chatHistoryService;
    }

    public async Task<Message> Chat(string message, Guid? id, CancellationToken ct = default)
    {
        // Generate a unique id (or use supplied id) so we can track the conversation
        var chatId = id ?? Guid.NewGuid();

        // If id is supplied, see if we have an ongoing request
        if (id is not null)
        {
            var chatHistory = await _chatHistoryService.GetChatHistory(chatId, ct);
            if (!chatHistory.Any())
            {
                return new Message(id.Value, CHAT_NOT_FOUND, ChatRole.System, DateTimeOffset.UtcNow);
            }

            // Check if the chat is awaiting a response (last entry will be the user message)
            var lastChatHistoryEntry = chatHistory.Last();
            var thinkingText = CheckThinking(lastChatHistoryEntry);

            if (thinkingText is not null)
            {
                return new Message(chatId, thinkingText, ChatRole.System, DateTimeOffset.UtcNow);
            }
        }

        // Start new or append to chat conversation
        var newChatHistory = await _chatHistoryService.AddToChatHistory(chatId, ChatRole.User, message, ct);

        // Delegate request to ChatProcessor - Need to figure out how to determine if channel is full and can't accept more processing requests.
        await _chatChannel.Writer.WriteAsync(new ChatChannelRequest(chatId, newChatHistory));

        return new Message(chatId, $"{THINKING_ABOUT} {message}", ChatRole.Assistant, DateTimeOffset.UtcNow);
    }

    public async Task<Message> GetLastResponse(Guid id, CancellationToken ct = default)
    {
        var chatHistory = await _chatHistoryService.GetChatHistory(id, ct);
        if (!chatHistory.Any())
        {
            return new Message(id, "No chat history", ChatRole.System, DateTimeOffset.UtcNow);
        }

        // Check if the chat is awaiting a response (last entry will be the user message)
        var lastChatHistoryEntry = chatHistory.Last();
        var thinkingText = CheckThinking(lastChatHistoryEntry);

        return thinkingText is not null
            ? new Message(id, thinkingText, ChatRole.System, DateTimeOffset.UtcNow)
            : lastChatHistoryEntry.ToChatMessageSummary(id);
    }

    public async Task<Conversation> GetConversation(Guid id, CancellationToken ct)
    {
        var chatHistory = await _chatHistoryService.GetChatHistory(id, ct);
        if (!chatHistory.Any())
        {
            return new Conversation
                (
                    new List<Message> { new Message(id, "No chat history", ChatRole.System, DateTimeOffset.UtcNow) },
                    false
                );
        }

        var messages = chatHistory.ConvertAll(m => m.ToChatMessageSummary(id));

        string? thinkingText = CheckThinking(chatHistory.Last());
        return (thinkingText is null)
            ? new Conversation(messages, false)
            : new Conversation
            (
                [.. messages, new Message(id, thinkingText, ChatRole.System, DateTimeOffset.UtcNow)],
                true
            );
    }

    #region Private Methods

    private static string? CheckThinking(ChatMessage? lastChatHistoryEntry)
    {
        if (lastChatHistoryEntry is null)
        {
            return CHAT_NOT_FOUND;
        }
        else if (lastChatHistoryEntry.Role == ChatRole.User)
        {
            return $"{STILL_THINKING_ABOUT} {lastChatHistoryEntry.Text}";
        }

        return null;
    }

    #endregion Private Methods
}
