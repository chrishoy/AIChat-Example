
using Microsoft.Extensions.AI;
using System.Text;
using System.Threading.Channels;

namespace OllamaChatClientExample.Server.Chat;

/// <summary>
/// Processes a <see cref="Channel<T>" requests/> to make a long-running call to a <see cref="IChatClient"/>
/// </summary>
public class ChatProcessor : BackgroundService
{
    private readonly Channel<ChatChannelRequest> _chatChannel;
    private readonly IChatClient _chatClient;
    private readonly IChatHistoryService _chatHistoryService;

    public ChatProcessor(Channel<ChatChannelRequest> chatChannel, IChatClient chatClient, IChatHistoryService chatHistoryService)
    {
        _chatChannel = chatChannel;
        _chatClient = chatClient;
        _chatHistoryService = chatHistoryService;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // Read any request placed in the channel and forward to chat processing logic
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
