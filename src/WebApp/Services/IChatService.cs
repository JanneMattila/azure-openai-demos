using WebApp.Models;

namespace WebApp.Services
{
    public interface IChatService
    {
        Task<string> SetPrompt(string userID, Prompt prompt);

        Task<string> GetResponseAsync(string userID, string text);
    }
}