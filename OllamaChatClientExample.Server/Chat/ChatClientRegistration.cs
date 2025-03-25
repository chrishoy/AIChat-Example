using Microsoft.Extensions.AI;

namespace OllamaChatClientExample.Server.Chat;

public static class ChatClientRegistration
{
    public static IServiceCollection AddChatClient(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("ChatSettings");

        var useEmulator = section.GetValue<bool>("UseEmulator");
        if (useEmulator)
        {
            services.AddChatClient(new ChatClientEmulator(section.GetValue<int>("EmulatorResponseTime")));
        }
        else
        {
            string endpoint = section.GetValue<string>("OllamaEndpoint")!;
            string model = section.GetValue<string>("OllamaModel")!;
            services.AddChatClient(new OllamaChatClient(new Uri(endpoint), model));
        }
        return services;
    }
}
