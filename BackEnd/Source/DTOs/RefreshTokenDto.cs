using System.ComponentModel.DataAnnotations;

namespace Source.DTOs
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Refresh token là bắt buộc")]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class LogoutDto
    {
        [Required(ErrorMessage = "User ID là bắt buộc")]
        public string UserId { get; set; } = string.Empty;

        public string? DeviceInfo { get; set; }
    }
}