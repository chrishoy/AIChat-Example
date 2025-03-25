using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;
using ZiggyCreatures.Caching.Fusion;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using OllamaChatClientExample.Server.Chat;
using OllamaChatClientExample.Server.Weather;
using System.Threading.Channels;

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
    Channel.CreateBounded<ChatChannelRequest>(new BoundedChannelOptions(capacity: 1)
    {
        FullMode = BoundedChannelFullMode.DropWrite,
        SingleReader = true,
        SingleWriter = false,
        AllowSynchronousContinuations = false
    }));

// Add FusionCache using Redis as Hybrid (in-memory + distributed) cache
builder.Services
    .AddFusionCache()   
    .WithDefaultEntryOptions(options => options.Duration = TimeSpan.FromMinutes(15))
    .WithSerializer(new FusionCacheSystemTextJsonSerializer())
    .WithDistributedCache(new RedisCache(new RedisCacheOptions { Configuration = "localhost:6379" }))
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
