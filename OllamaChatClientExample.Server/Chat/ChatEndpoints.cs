﻿namespace OllamaChatClientExample.Server.Chat;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/chat");

        group.MapPost("/", Chat).WithName("Chat");
        group.MapGet("/{id:Guid}", GetReply).WithName("GetChat");
        group.MapPost("/{id:Guid}", ContinueChat).WithName("ContinueChat");
        group.MapGet("/conversation/{id:Guid}", GetConversation).WithName("GetConversation");
        group.MapGet("/clear/{id:Guid}", ClearChat).WithName("ClearChat");
    }

    private static async Task<IResult> Chat(ChatRequest request, IChatService chatService, CancellationToken ct)
    {
        var response = await chatService.Chat(request.Message, request.Id, ct);
        return response is not null ? Results.Ok(response) : Results.NotFound();
    }

    private static async Task<IResult> GetReply(Guid id, IChatService chatService, CancellationToken ct)
    {
        var response = await chatService.GetLastResponse(id, ct);
        return response is not null ? Results.Ok(response) : Results.NotFound();
    }

    private static async Task<IResult> GetConversation(Guid id, IChatService chatService, CancellationToken ct)
    {
        var conversation = await chatService.GetConversation(id, ct);
        return conversation is not null ? Results.Ok(new ChatHistoryResponse(conversation)) : Results.NotFound();
    }

    private static async Task<IResult> ContinueChat(Guid id, AddToChatRequest request, IChatService chatService, CancellationToken ct)
    {
        var reply = await chatService.ContinueChat(id, request.Message, ct);
        return reply is not null ? Results.Ok(new ChatResponse(reply, id)) : Results.NotFound();
    }

    private static async Task<IResult> ClearChat(Guid id, IChatHistoryService chatHistoryService, CancellationToken ct)
    {
        await chatHistoryService.Clear(id, ct);
        return Results.Ok(new ChatHistoryClearedResponse(true));
    }
}
