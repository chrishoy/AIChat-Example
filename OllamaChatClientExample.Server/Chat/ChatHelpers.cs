namespace OllamaChatClientExample.Server.Chat;

public static class ChatHelpers
{
    public static string BuildChatTag(Guid id) => $"ChatService.Chat({id})";

    public static string BuildChatHistoryCacheKey(Guid id) => $"ChatService.ChatHistory({id})";
}
