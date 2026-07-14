using Source.DTOs.Admin;

namespace Source.Service.Admin.Interface
{
    public interface IAdminQuizService
    {
        // Quiz CRUD
        Task<List<AdminQuizDto>> GetAllQuizzes();
        Task<AdminQuizDto?> GetQuizById(int id);
        Task<AdminQuizDto> CreateQuiz(CreateQuizDto dto);
        Task<AdminQuizDto?> UpdateQuiz(int id, UpdateQuizDto dto);
        Task<bool> DeleteQuiz(int id);
    }
}