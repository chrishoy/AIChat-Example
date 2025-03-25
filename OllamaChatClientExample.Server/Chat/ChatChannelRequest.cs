using Microsoft.Extensions.AI;

namespace OllamaChatClientExample.Server.Chat;

public record ChatChannelRequest(Guid Id, IEnumerable<ChatMessage> ChatHistory) {}
