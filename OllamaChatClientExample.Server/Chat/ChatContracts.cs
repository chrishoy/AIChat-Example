using Microsoft.Extensions.AI;

namespace OllamaChatClientExample.Server.Chat;

public record NewChatRequest(string Message);
public record NewChatResponse(Guid Id);
public record AddToChatRequest(string Message);
public record ChatResponse(string Reply);
public record ChatHistoryResponse(IEnumerable<ChatMessageSummary> Conversation);
public record ChatHistoryClearedResponse(bool Cleared);
public record ChatMessageSummary(string Text, ChatRole Role, DateTimeOffset? Timestamp);
