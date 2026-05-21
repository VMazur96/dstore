using Drajbot.Api.DTOs.Auth;

namespace Drajbot.Api.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(UserRegisterDto request);
        Task<string> LoginAsync(UserLoginDto request);
    }
}