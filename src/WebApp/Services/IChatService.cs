using WebApp.Models;

namespace WebApp.Services
{
    public interface IChatService
    {
        Task<string> SetPrompt(Prompt prompt);

        Task<string> GetResponseAsync(string text);
    }
}