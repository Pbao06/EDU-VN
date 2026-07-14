using Source.DTOs;

namespace Source.Service.Interface
{
    public interface ISubjectService
    {
        /// <summary>
        /// Lấy chi tiết subject với Topics list (không có Questions)
        /// </summary>
        Task<SubjectDetailDto> GetSubjectDetail(int learningPathId, int subjectId, string userId);
    }
}
