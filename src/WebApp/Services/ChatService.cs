using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Options;
using WebApp.Models;

namespace WebApp.Services;

public class ChatService : IChatService
{
    private readonly ILogger _logger;
    private readonly ChatOptions _options;
    private readonly OpenAIClient _client;
    private ChatCompletionsOptions _chatCompletionsOptions;

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

        _chatCompletionsOptions = new ChatCompletionsOptions
        {
            ChoicesPerPrompt = 1,
            MaxTokens = 800,
            Temperature = 0.7f,
            FrequencyPenalty = 0.0f,
            PresencePenalty = 0.0f,
            NucleusSamplingFactor = 0.95f // Top P
        };

        _chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, _options.Prompt));
    }

    public async Task<string> SetPrompt(Prompt prompt)
    {
        _chatCompletionsOptions.Messages.Clear();

        if (!string.IsNullOrEmpty(prompt.SystemMessage))
        {
            _chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, prompt.SystemMessage));
        }

        return await GetResponseAsync(prompt.UserMessage);
    }

    public async Task<string> GetResponseAsync(string text)
    {
        if (_options.Disabled)
        {
            await Task.Delay(1000);
            return $"Automated response to: {text}";
        }

        while (_chatCompletionsOptions.Messages.Count > 20) _chatCompletionsOptions.Messages.RemoveAt(0);

        _chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, text));

        var response = await _client.GetChatCompletionsAsync(_options.ModelName, _chatCompletionsOptions);

        var chatResponse = response.Value.Choices.First().Message.Content;
        _chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.Assistant, chatResponse));
        return chatResponse;
    }
}
