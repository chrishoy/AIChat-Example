using Microsoft.Extensions.AI;

namespace OllamaChatClientExample.Server.Chat;

public record ChatRequest(string Message, Guid? Id = null);
public record ChatResponse(string Reply, Guid Id);
public record ChatHistoryResponse(IEnumerable<ChatMessageSummary> Conversation);
public record ChatHistoryClearedResponse(bool Cleared);
public record ChatMessageSummary(Guid Id, string Text, ChatRole Role, DateTimeOffset? Timestamp);
