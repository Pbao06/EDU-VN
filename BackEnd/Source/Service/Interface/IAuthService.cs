using Source.DTOs;
using Source.Models;

namespace Source.Service.Interface
{
    public interface IAuthService
    {
        Task<RegisterDto> RegisterUser(RegisterDto model);
        Task<AuthResponseDto> Login(LoginDto model, string? deviceInfo = null);
        Task<string> GenerateToken(User user);
        Task<string> GenerateRefreshToken(User user, string? deviceInfo = null);
        Task<AuthResponseDto> RefreshToken(string refreshToken);
        Task Logout(string userId, string? deviceInfo = null);
        Task RevokeAllRefreshTokens(string userId);
    }
}
