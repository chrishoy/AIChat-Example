namespace OllamaChatClientExample.Server.Chat;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/chat");

        group.MapPost("/", NewChat).WithName("NewChat");
        group.MapGet("/{id:Guid}", GetReply).WithName("GetChat");
        group.MapPost("/{id:Guid}", ContinueChat).WithName("ContinueChat");
        group.MapGet("/conversation/{id:Guid}", GetConversation).WithName("GetConversation");
        group.MapGet("/clear/{id:Guid}", ClearChat).WithName("ClearChat");
    }

    private static async Task<IResult> NewChat(NewChatRequest request, IChatService chatService)
    {
        var id = await chatService.NewChat(request.Message);
        return id != Guid.Empty ? Results.Ok(new NewChatResponse(id)) : Results.NotFound();
    }

    private static async Task<IResult> GetReply(Guid id, IChatService chatService)
    {
        var reply = await chatService.GetReply(id);
        return reply is not null ? Results.Ok(new ChatResponse(reply)) : Results.NotFound();
    }

    private static async Task<IResult> GetConversation(Guid id, IChatService chatService)
    {
        var conversation = await chatService.GetConversation(id);
        return conversation is not null ? Results.Ok(new ChatHistoryResponse(conversation)) : Results.NotFound();
    }

    private static async Task<IResult> ContinueChat(Guid id, AddToChatRequest request, IChatService chatService)
    {
        var reply = await chatService.ContinueChat(id, request.Message);
        return reply is not null ? Results.Ok(new ChatResponse(reply)) : Results.NotFound();
    }

    private static async Task<IResult> ClearChat(Guid id, IChatService chatService)
    {
        await chatService.Clear(id);
        return Results.Ok();
    }
}
