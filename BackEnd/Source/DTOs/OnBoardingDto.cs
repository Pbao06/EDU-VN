using System.ComponentModel.DataAnnotations;

namespace Source.DTOs
{
    /// <summary>
    /// DTO chứa thông tin onboarding của người dùng
    /// </summary>
    public class OnBoardingDto
    {
        /// <summary>
        /// Họ tên đầy đủ của người dùng
        /// </summary>
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MinLength(2, ErrorMessage = "Họ tên phải có ít nhất 2 ký tự")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Loại người dùng: HocSinh, SinhVien, NguoiDiLam
        /// </summary>
        [Required(ErrorMessage = "Loại người dùng là bắt buộc")]
        public string UserType { get; set; } = string.Empty;

        /// <summary>
        /// Mục tiêu chính: ThiDaiHoc, HocLapTrinh, ThayDoiNghiep...
        /// </summary>
        [Required(ErrorMessage = "Mục tiêu là bắt buộc")]
        public string? MainGoal { get; set; }

        /// <summary>
        /// ID của lĩnh vực ưu thích (FK vào bảng Field)
        /// </summary>
        [Required(ErrorMessage = "Lĩnh vực ưu thích là bắt buộc")]
        public int? FieldId { get; set; } // Bắt buộc để quiz đưa ra các câu hỏi về lĩnh vực cụ thể
    }

    /// <summary>
    /// DTO phản hồi trạng thái onboarding
    /// </summary>
    public class OnboardingStatusDto
    {
        /// <summary>
        /// Người dùng đã hoàn thành onboarding chưa
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Thông tin onboarding nếu đã hoàn thành
        /// </summary>
        public OnBoardingDto? OnboardingData { get; set; }
    }
}
