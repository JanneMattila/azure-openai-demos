using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using WebApp.Models;

namespace WebApp.Services;

public class ChatService : IChatService
{
    private readonly ILogger _logger;
    private readonly ChatOptions _options;
    private readonly OpenAIClient _client;
    private readonly MemoryCache _users = new(new MemoryCacheOptions()
    {
        ExpirationScanFrequency = TimeSpan.FromMinutes(10)
    });

    public ChatService(ILoggerFactory loggerFactory, IOptions<ChatOptions> options)
    {
        _logger = loggerFactory.CreateLogger<ChatService>();
        _options = options.Value;

        if (string.IsNullOrEmpty(_options.Key))
        {
            _client = new OpenAIClient(new Uri(_options.Endpoint), new DefaultAzureCredential());
        }
        else
        {
            _client = new OpenAIClient(new Uri(_options.Endpoint), new AzureKeyCredential(_options.Key));
        }
    }

    private ChatCompletionsOptions GetUserChatCompletionsOptions(string userID)
    {
        if (_users.TryGetValue<ChatCompletionsOptions>(userID, out var chatCompletionsOptions))
        {
            return chatCompletionsOptions;
        }
        else
        {
            chatCompletionsOptions = new ChatCompletionsOptions
            {
                ChoicesPerPrompt = 1,
                MaxTokens = 800,
                Temperature = 0.7f,
                FrequencyPenalty = 0.0f,
                PresencePenalty = 0.0f,
                NucleusSamplingFactor = 0.95f // Top P
            };
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, _options.Prompt));

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));
            _users.Set(userID, chatCompletionsOptions, cacheEntryOptions);
            return chatCompletionsOptions;
        }
    }

    public async Task<string> SetPrompt(string userID, Prompt prompt)
    {
        var chatCompletionsOptions = GetUserChatCompletionsOptions(userID);
        chatCompletionsOptions.Messages.Clear();

        if (!string.IsNullOrEmpty(prompt.SystemMessage))
        {
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, prompt.SystemMessage));
        }

        return await GetResponseAsync(userID, prompt.UserMessage);
    }

    public async Task<string> GetResponseAsync(string userID, string text)
    {
        if (_options.Disabled)
        {
            await Task.Delay(1000);
            return $"Automated response to: {text}";
        }

        var chatCompletionsOptions = GetUserChatCompletionsOptions(userID);
        while (chatCompletionsOptions.Messages.Count > 20) chatCompletionsOptions.Messages.RemoveAt(0);

        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }
        else
        {
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, text));
            var response = await _client.GetChatCompletionsAsync(_options.ModelName, chatCompletionsOptions);

            var chatResponse = response.Value.Choices.First().Message.Content;
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.Assistant, chatResponse));
            return chatResponse;
        }
    }
}
