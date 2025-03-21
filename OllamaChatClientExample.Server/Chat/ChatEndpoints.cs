namespace OllamaChatClientExample.Server.Chat;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/chat");

        group.MapPost("/", NewChat).WithName("NewChat");
        group.MapGet("/{id:Guid}", GetReply).WithName("GetChat");
        group.MapPost("/{id:Guid}", AddToChat).WithName("AddToChat");
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

    private static async Task<IResult> AddToChat(Guid id, AddToChatRequest request, IChatService chatService)
    {
        var reply = await chatService.AddToChat(id, request.Message);
        return reply is not null ? Results.Ok(new ChatResponse(reply)) : Results.NotFound();
    }

    private static async Task<IResult> ClearChat(Guid id, IChatService chatService)
    {
        await chatService.Clear(id);
        return Results.Ok();
    }
}
