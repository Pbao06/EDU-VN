using Source.DTOs.Admin;

namespace Source.Service.Admin.Interface
{
    public interface IAdminLearningQuestionService
    {
        // LearningQuestion CRUD
        Task<List<AdminLearningQuestionDto>> GetAllLearningQuestions();
        Task<AdminLearningQuestionDto?> GetLearningQuestionById(int id);
        Task<AdminLearningQuestionDto> CreateLearningQuestion(CreateLearningQuestionDto dto);
        Task<AdminLearningQuestionDto?> UpdateLearningQuestion(int id, UpdateLearningQuestionDto dto);
        Task<bool> DeleteLearningQuestion(int id);

       
    }
}