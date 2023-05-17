namespace WebApp.Models;

public class ChatOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public bool Disabled { get; set; } = false;
}
