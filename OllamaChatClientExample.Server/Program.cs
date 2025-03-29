using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;
using ZiggyCreatures.Caching.Fusion;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using OllamaChatClientExample.Server.Chat;
using OllamaChatClientExample.Server.Weather;
using System.Threading.Channels;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddChatClient(builder.Configuration);
builder.Services.AddHostedService<ChatProcessor>();

builder.Services.AddSingleton<IWeatherService, WeatherService>();
builder.Services.AddTransient<IChatHistoryService, ChatHistoryService>();
builder.Services.AddTransient<IChatService, ChatService>();

// See https://learn.microsoft.com/en-us/dotnet/core/extensions/channels#bounded-creation-patterns
builder.Services.AddSingleton(_ =>
    // Set channel capacity to 1 to ensure that only one request is processed at a time
    Channel.CreateBounded<ChatChannelRequest>(new BoundedChannelOptions(capacity: 1)
    {
        FullMode = BoundedChannelFullMode.DropWrite,
        SingleReader = true,
        SingleWriter = false,
        AllowSynchronousContinuations = false
    }));

// Add FusionCache using Redis as Hybrid (in-memory + distributed) cache
var chatHistoryEvictionDuration = builder.Configuration.GetValue<TimeSpan>("ChatSettings:ChatHistoryEvictionDuration");
var cacheEndpoint = builder.Configuration.GetValue<string>("ChatSettings:RedisEndpoint");
IDistributedCache cache = new RedisCache(new RedisCacheOptions { Configuration = cacheEndpoint });
builder.Services
    .AddFusionCache()   
    .WithDefaultEntryOptions(options => options.Duration = chatHistoryEvictionDuration)
    .WithSerializer(new FusionCacheSystemTextJsonSerializer())
    .WithDistributedCache(cache)
.AsHybridCache();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapChatEndpoints();
app.MapWeatherEndpoints();

app.MapFallbackToFile("/index.html");

app.Run();
