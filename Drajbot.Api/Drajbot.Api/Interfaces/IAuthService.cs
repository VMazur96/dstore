using Drajbot.Api.DTOs.Auth;

namespace Drajbot.Api.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(UserRegisterDto request);
        Task<string> LoginAsync(UserLoginDto request);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<string> ResetPasswordAsync(ResetPasswordDto request);
        Task<string> DeleteAccountAsync(int userId, string password);
        Task<string> AdminForceChangeCredentialsAsync(int targetUserId, string? newEmail, string? newPassword);
    }
}