using Optiviera.Models;

namespace Optiviera.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(WaveUser user);
        Task<(bool Success, string Token, WaveUser? User, string Message)> LoginAsync(string email, string password);
        Task<(bool Success, string Token, WaveUser? User, string Message)> RegisterAsync(string companyName, string email, string password, string firstName, string lastName);
    }
}
