using Source.DTOs;

namespace Source.Service.Interface
{
    public interface ILearningPathService
    {
        // ==================== LEARNING PATH LEVEL ====================

        /// <summary>
        /// Tạo learning path mới cho user với career đã chọn
        /// </summary>
        Task<CreateLearningPathResponseDto> StartLearningPath(string userId, int careerId, string? title = null);

        /// <summary>
        /// Lấy tất cả learning paths của user (summary only)
        /// </summary>
        Task<List<LearningPathDto>> GetUserLearningPaths(string userId);

        /// <summary>
        /// Lấy chi tiết learning path với Subjects list (không có Topics)
        /// </summary>
        Task<LearningPathDetailDto> GetLearningPathDetail(int learningPathId, string userId);
    }
}
