using Microsoft.EntityFrameworkCore;
using Source.Data;
using Source.DTOs;
using Source.Middleware;
using Source.Models;
using Source.Models.Enums;
using Source.Service.Interface;

namespace Source.Service
{
    /// <summary>
    /// Service xử lý logic liên quan đến onboarding người dùng
    /// Class này chứa logic nghiệp vụ, tách biệt khỏi Controller
    /// </summary>
    public class OnboardingService : IOnboardingService
    {
        // Database context để thao tác với database
        private readonly ApplicationDbContext _context;

        // Constructor injection - pattern tốt để test và maintain
        public OnboardingService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hoàn thành quá trình onboarding cho người dùng
        /// Cập nhật thông tin user và đánh dấu onboarding đã hoàn thành
        /// </summary>
        public async Task CompleteOnboardingAsync(string userId, OnBoardingDto dto)
        {
            // Bước 1: Tìm user trong database theo userId
            var user = await _context.Users.FindAsync(userId);

            // Bước 2: Kiểm tra xem user có tồn tại không
            if (user == null)
            {
                // Nếu không tìm thấy user, ném exception
                // Middleware sẽ bắt exception này và trả về lỗi 400
                throw new BadRequestException("Không tìm thấy người dùng");
            }

            // Bước 3: Kiểm tra xem user đã hoàn thành onboarding chưa
            if (user.IsOnboardingCompleted)
            {
                // Nếu đã hoàn thành, ném exception để tránh update lại
                throw new BadRequestException("Người dùng đã hoàn thành onboarding");
            }

            // Bước 4: Cập nhật thông tin user từ DTO
            user.FullName = dto.FullName;
            
            // Parse UserType từ string sang enum
            if (Enum.TryParse<UserType>(dto.UserType, out var userType))
            {
                user.UserType = userType;
            }
            
            // Parse MainGoal từ string sang enum
            if (!string.IsNullOrEmpty(dto.MainGoal) && Enum.TryParse<MainGoal>(dto.MainGoal, out var mainGoal))
            {
                user.MainGoal = mainGoal;
            }
            
            // Lưu FieldId (đã là int)
            user.FieldId = dto.FieldId;
            
            user.IsOnboardingCompleted = true;  // Đánh dấu đã hoàn thành onboarding
            user.UpdatedAt = DateTime.UtcNow;  // Cập nhật thời gian sửa đổi

            // Bước 5: Lưu thay đổi vào database
            await _context.SaveChangesAsync();

            // Không cần return gì vì phương thức là void (Task)
        }

        /// <summary>
        /// Kiểm tra xem người dùng đã hoàn thành onboarding chưa
        /// Trả về thông tin trạng thái và dữ liệu onboarding nếu đã hoàn thành
        /// </summary>
        public async Task<OnboardingStatusDto> IsOnboardingCompletedAsync(string userId)
        {
            // Bước 1: Tìm user trong database theo userId
            var user = await _context.Users.FindAsync(userId);

            // Bước 2: Kiểm tra xem user có tồn tại không
            if (user == null)
            {
                // Nếu không tìm thấy user, ném exception
                throw new BadRequestException("Không tìm thấy người dùng");
            }

            // Bước 3: Tạo đối tượng phản hồi trạng thái
            var statusDto = new OnboardingStatusDto
            {
                // Gán trạng thái onboarding
                IsCompleted = user.IsOnboardingCompleted
            };
            // Bước 4: Nếu đã hoàn thành onboarding, thêm dữ liệu chi tiết
            if (user.IsOnboardingCompleted)
            {
                statusDto.OnboardingData = new OnBoardingDto
                {
                    FullName = user.FullName,
                    UserType = user.UserType?.ToString() ?? string.Empty,
                    MainGoal = user.MainGoal?.ToString(),
                    FieldId = user.FieldId
                };
            }

            // Bước 5: Trả về kết quả
            return statusDto;
        }
    }
}