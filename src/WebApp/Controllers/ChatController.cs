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
        var response = await _chatService.GetResponseAsync(request.User, request.Text);
        return Ok(new ChatModel()
        {
            Text = response
        });
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] ChatOptionModel request)
    {
        if (request.Option < 0 || _options.Prompts.Count - 1 < request.Option)
        {
            return BadRequest();
        }
        var response = await _chatService.SetPrompt(request.User, _options.Prompts[request.Option]);
        return Ok(new ChatModel()
        {
            Text = response
        });
    }
}
