using Microsoft.Extensions.AI;

namespace OllamaChatClientExample.Server.Chat;

public record ChatRequest(string Message, Guid? Id = null);
public record ChatResponse(string Reply, Guid Id);
public record ConversationClearedResponse(bool Cleared);
public record Message(Guid Id, string Text, ChatRole Role, DateTimeOffset? Timestamp);
public record Conversation(IEnumerable<Message> Messages, bool? Busy);
