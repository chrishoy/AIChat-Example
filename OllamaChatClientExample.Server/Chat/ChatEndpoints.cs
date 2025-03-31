namespace OllamaChatClientExample.Server.Chat;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/chat");

        group.MapPost("/", Chat).WithName("Chat");
        group.MapPost("/{id:Guid}", ContinueChat).WithName("ContinueChat");
        group.MapGet("/{id:Guid}", GetLastResponse).WithName("GetChat");
        group.MapGet("/conversation/{id:Guid}", GetConversation).WithName("GetConversation");
        group.MapGet("/clear/{id:Guid}", ClearChat).WithName("ClearChat");
    }

    private static async Task<IResult> Chat(ChatRequest request, IChatService chatService, CancellationToken ct)
    {
        var response = await chatService.Chat(request.Message, request.Id, ct);
        return response is not null ? Results.Ok(response) : Results.NotFound();
    }

    private static async Task<IResult> ContinueChat(Guid id, ChatRequest request, IChatService chatService, CancellationToken ct)
    {
        var response = await chatService.Chat(request.Message, id, ct);
        return response is not null ? Results.Ok(response) : Results.NotFound();
    }

    private static async Task<IResult> GetLastResponse(Guid id, IChatService chatService, CancellationToken ct)
    {
        var response = await chatService.GetLastResponse(id, ct);
        return response is not null ? Results.Ok(response) : Results.NotFound();
    }

    private static async Task<IResult> GetConversation(Guid id, IChatService chatService, CancellationToken ct)
    {
        var conversation = await chatService.GetConversation(id, ct);
        return conversation is not null ? Results.Ok(conversation) : Results.NotFound();
    }

    private static async Task<IResult> ClearChat(Guid id, IChatHistoryService chatHistoryService, CancellationToken ct)
    {
        await chatHistoryService.Clear(id, ct);
        return Results.Ok(new ConversationClearedResponse(true));
    }
}
