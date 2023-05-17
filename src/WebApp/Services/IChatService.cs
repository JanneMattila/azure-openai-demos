namespace WebApp.Services
{
    public interface IChatService
    {
        Task<string> GetResponseAsync(string text);
    }
}