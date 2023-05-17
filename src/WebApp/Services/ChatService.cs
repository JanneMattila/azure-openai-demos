using Microsoft.Extensions.Options;
using WebApp.Models;

namespace WebApp.Services;

public class ChatService : IChatService
{
    private readonly ILogger _logger;
    private readonly ChatOptions _options;

    public ChatService(ILoggerFactory loggerFactory, IOptions<ChatOptions> options)
    {
        _logger = loggerFactory.CreateLogger<ChatService>();
        _options = options.Value;
    }

    public async Task<string> GetResponseAsync(string text)
    {
        return await Task.FromResult(text);
    }
}
