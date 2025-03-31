using Microsoft.Extensions.AI;

namespace OllamaChatClientExample.Server.Chat;

public static class ChatHelpers
{
    public static string ToChatTag(this Guid id) => $"ChatService.Chat({id})";

    public static string ToChatHistoryCacheKey(this Guid id) => $"ChatService.ChatHistory({id})";

    public static Message ToChatMessageSummary(this ChatMessage chatMessage, Guid id)
    {
        DateTimeOffset? timestamp = null;
        var props = chatMessage.AdditionalProperties;
        if (props is not null && props.ContainsKey("Timestamp"))
        {
            timestamp = (DateTimeOffset?)props["Timestamp"];
        }

        return new Message(id, chatMessage.Text, chatMessage.Role, timestamp);
    }
}
