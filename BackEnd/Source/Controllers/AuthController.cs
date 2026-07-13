using Microsoft.AspNetCore.Mvc;
using Source.DTOs;
using Source.Service;
using Source.Service.Interface;

namespace Source.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // nem ve errror 
            }

            var result = await _authService.RegisterUser(model);
            return Success(result, "Đăng ký thành công");
        }

        /// <summary>
        /// Đăng nhập và nhận tokens
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy device info từ header (nếu có)
            var deviceInfo = Request.Headers["User-Agent"].ToString();
            var result = await _authService.Login(model, deviceInfo);
            return Success(result, "Đăng nhập thành công");
        }

        /// <summary>
        /// Làm mới access token bằng refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RefreshToken(model.RefreshToken);
            return Success(result, "Làm mới token thành công");
        }

        /// <summary>
        /// Đăng xuất (hủy refresh token)
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _authService.Logout(model.UserId, model.DeviceInfo);
            return Success(null, "Đăng xuất thành công");
        }

        /// <summary>
        /// Hủy tất cả refresh tokens của user
        /// </summary>
        [HttpPost("revoke-all")]
        public async Task<IActionResult> RevokeAllTokens([FromBody] LogoutDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _authService.RevokeAllRefreshTokens(model.UserId);
            return Success(null, "Đã hủy tất cả tokens");
        }
    }
}