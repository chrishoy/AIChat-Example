
using Microsoft.Extensions.AI;
using System.Text;
using System.Threading.Channels;

namespace OllamaChatClientExample.Server.Chat;

public class ChatProcessor : BackgroundService
{
    private readonly IChatClient _chatClient;
    private readonly IChatHistoryService _chatHistoryService;
    private readonly Channel<ChatChannelRequest> _chatChannel;

    public ChatProcessor(IChatClient chatClient, IChatHistoryService chatHistoryService, Channel<ChatChannelRequest> chatChannel)
    {
        _chatClient = chatClient;
        _chatHistoryService = chatHistoryService;
        _chatChannel = chatChannel;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // Read from the channel and process the chat
        while (await _chatChannel.Reader.WaitToReadAsync(ct))
        {
            var request = await _chatChannel.Reader.ReadAsync(ct);
            await ProcessChat(request, ct);
        }
    }

    private async Task ProcessChat(ChatChannelRequest request, CancellationToken ct)
    {
        // Background task that performs the long-running chat operation
        var chatCompletionResponse = await _chatClient.GetResponseAsync(request.ChatHistory, cancellationToken: ct);

        var chatResponseBuilder = new StringBuilder();
        foreach (var messageResponse in chatCompletionResponse.Messages)
        {
            chatResponseBuilder.AppendLine(messageResponse.Text);
        }

        var chatResponse = chatResponseBuilder.ToString();
        await _chatHistoryService.AddToChatHistory(request.Id, ChatRole.Assistant, chatResponse, ct);
    }
}
