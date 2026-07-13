using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Source.Data;
using Source.DTOs;
using Source.Middleware;
using Source.Models;
using Source.Service.Interface;
using System.ComponentModel.DataAnnotations;



namespace Source.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<RegisterDto> RegisterUser(RegisterDto model)
        {
            // flow validation -> create newUser -> save -> reutrn contorller
            // kiem tra exist mail ?
            var existEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existEmail != null) throw new BadRequestException("Email đã tồn tại");

            var newUser = new User
            {
                Email = model.Email,
                UserName = model.Email, // IdentityUser yêu cầu UserName phải có giá trị hợp lệ
                FullName = model.FullName,
                IsOnboardingCompleted = false,
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);// auto hash password and save to db
            if (!result.Succeeded)
            {
                throw new ValidationException("Đăng ký thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            }
            return new RegisterDto
            {
                Email = newUser.Email,
                FullName = newUser.FullName,
            };

        }
        public async Task<AuthResponseDto> Login(LoginDto model, string? deviceInfo = null)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                throw new BadRequestException("Email hoặc mật khẩu không đúng");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                throw new BadRequestException("Email hoặc mật khẩu không đúng");
            }

            var accessToken = GenerateToken(user);
            var refreshToken = await GenerateRefreshToken(user, deviceInfo);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Email = user.Email,
                FullName = user.FullName,
                UserId = user.Id
            };
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task<string> GenerateRefreshToken(User user, string? deviceInfo = null)
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token hết hạn sau 7 ngày
                DeviceInfo = deviceInfo
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken.Token;
        }

        public async Task<AuthResponseDto> RefreshToken(string refreshToken)
        {
            var existingToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken); // join vao table refreshtoken xem co refreshtoekn nao trung ko

            if (existingToken == null)
            {
                throw new BadRequestException("Refresh token không hợp lệ");
            }

            if (existingToken.IsRevoked)
            {
                throw new BadRequestException("Refresh token đã bị revoke");
            }

            if (existingToken.IsExpired)
            {
                throw new BadRequestException("Refresh token đã hết hạn");
            }

            // Revoke token cũ và tạo token mới
            existingToken.RevokedAt = DateTime.UtcNow;
            var newRefreshToken = await GenerateRefreshToken(existingToken.User, existingToken.DeviceInfo);
            existingToken.ReplacedByToken = newRefreshToken;

            var accessToken = GenerateToken(existingToken.User);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                Email = existingToken.User.Email,
                FullName = existingToken.User.FullName,
                UserId = existingToken.User.Id
            };
        }

        public async Task Logout(string userId, string? deviceInfo = null)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new BadRequestException("User không tồn tại");
            }

            var query = _context.RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked);

            // Nếu có deviceInfo, chỉ revoke token của device đó
            if (!string.IsNullOrEmpty(deviceInfo))
            {
                query = query.Where(rt => rt.DeviceInfo == deviceInfo);
            }

            var tokensToRevoke = await query.ToListAsync();
            foreach (var token in tokensToRevoke)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllRefreshTokens(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new BadRequestException("User không tồn tại");
            }

            var tokensToRevoke = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokensToRevoke)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

    }
}