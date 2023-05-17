using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IChatService _chatService;
    private readonly ChatPromptOptions _options;

    public ChatController(ILoggerFactory loggerFactory, IChatService chatService, IOptions<ChatPromptOptions> options)
    {
        _logger = loggerFactory.CreateLogger<ChatController>();
        _chatService = chatService;
        _options = options.Value;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_options);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ChatModel request)
    {
        var response = await _chatService.GetResponseAsync(request.Text);
        return Ok(new ChatModel()
        {
            Text = response
        });
    }

    [HttpPut("{index:int}")]
    public async Task<IActionResult> Put(int index)
    {
        if (index < 0 || _options.Prompts.Count - 1 < index)
        {
            return BadRequest();
        }
        var response = await _chatService.SetPrompt(_options.Prompts[index]);
        return Ok(new ChatModel()
        {
            Text = response
        });
    }
}
