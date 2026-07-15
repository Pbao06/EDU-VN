using Source.DTOs.Admin;

namespace Source.Service.Admin.Interface
{
    public interface IAdminLearningAnswers
    {
        Task<List<AdminLearningAnswerDto>> GetAllLearningAnswers();
        Task<AdminLearningAnswerDto?> GetLearningAnswerById(int id);
        Task<AdminLearningAnswerDto> CreateLearningAnswer(CreateLearningAnswerDto dto);
        Task<AdminLearningAnswerDto?> UpdateLearningAnswer(int id, UpdateLearningAnswerDto dto);
        Task<bool> DeleteLearningAnswer(int id);
    }
}
