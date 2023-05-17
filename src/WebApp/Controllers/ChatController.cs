using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] ChatModel request)
    {
        return Ok(new ChatModel()
        {
            Text = $"Received message: {request.Text}"
        });
    }
}
