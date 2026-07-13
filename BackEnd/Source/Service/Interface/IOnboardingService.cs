using Source.DTOs;

namespace Source.Service.Interface
{
    /// <summary>
    /// Interface định nghĩa các phương thức cho Onboarding Service
    /// Interface giúp code dễ test và dễ mở rộng
    /// </summary>
    public interface IOnboardingService
    {
        /// <summary>
        /// Hoàn thành quá trình onboarding cho người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng cần hoàn thành onboarding</param>
        /// <param name="dto">Dữ liệu onboarding từ client</param>
        /// <returns>Task không trả về dữ liệu (void)</returns>
        Task CompleteOnboardingAsync(string userId, OnBoardingDto dto);

        /// <summary>
        /// Kiểm tra xem người dùng đã hoàn thành onboarding chưa
        /// </summary>
        /// <param name="userId">ID của người dùng cần kiểm tra</param>
        /// <returns>Đối tượng chứa trạng thái onboarding và dữ liệu nếu đã hoàn thành</returns>
        Task<OnboardingStatusDto> IsOnboardingCompletedAsync(string userId);
    }
}