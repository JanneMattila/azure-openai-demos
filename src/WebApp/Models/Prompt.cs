namespace WebApp.Models;

public class Prompt
{
    public string Title { get; set; } = string.Empty;
    public string SystemMessage { get; set; } = string.Empty;
    public string UserMessage { get; set; } = string.Empty;
}
