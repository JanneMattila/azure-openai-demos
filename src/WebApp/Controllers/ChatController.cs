using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IChatService _chatService;

    public ChatController(ILoggerFactory loggerFactory, IChatService chatService)
    {
        _logger = loggerFactory.CreateLogger<ChatController>();
        _chatService = chatService;
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
}
