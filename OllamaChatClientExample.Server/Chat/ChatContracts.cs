namespace OllamaChatClientExample.Server.Chat;

public record NewChatRequest(string Message);
public record NewChatResponse(Guid Id);
public record AddToChatRequest(string Message);
public record ChatResponse(string Reply);
public record ChatHistoryResponse(IEnumerable<string> Conversation);
public record ChatHistoryClearedResponse(bool Cleared);
