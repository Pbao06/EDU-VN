using Source.DTOs;

namespace Source.Service.Interface
{
    /// <summary>
    /// Interface định nghĩa các phương thức cho Quiz Service
    /// Quiz Service xử lý logic cho Recommendation Quiz System
    /// Logic: 1 field = 1 quiz duy nhất
    /// </summary>
    public interface IQuizService
    {
        /// <summary>
        /// Lấy quiz duy nhất của user theo field
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <returns>Quiz với câu hỏi và đáp án</returns>
        Task<QuizDto> GetUserQuizAsync(string userId);

        /// <summary>
        /// Nộp bài quiz và tính điểm career recommendation
        /// User có thể làm lại nhiều lần, mỗi lần lưu lịch sử
        /// </summary>
        /// <param name="userId">ID của user đang làm quiz</param>
        /// <param name="request">Danh sách đáp án user chọn</param>
        /// <returns>Kết quả career recommendation</returns>
        Task<QuizResultDto> SubmitQuizAsync(string userId, QuizSubmitRequestDto request);

        /// <summary>
        /// Lấy tất cả lịch sử kết quả quiz của user
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <returns>Tất cả lịch sử kết quả quiz</returns>
        Task<List<QuizResultDto>> GetQuizHistoryAsync(string userId);
    }
}
