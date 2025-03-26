using Microsoft.Extensions.AI;

namespace OllamaChatClientExample.Server.Chat;

public record ChatRequest(string Message, Guid? Id);
public record ChatResponse(string Reply, Guid Id);
public record AddToChatRequest(string Message);
public record ChatHistoryResponse(IEnumerable<ChatMessageSummary> Conversation);
public record ChatHistoryClearedResponse(bool Cleared);
public record ChatMessageSummary(Guid Id, string Text, ChatRole Role, DateTimeOffset? Timestamp);
