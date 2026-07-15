using Source.DTOs.Admin;

namespace Source.Service.Admin.Interface
{
    public interface IAdminRecoAnswers
    {
        Task<List<AdminRecommendationAnswerDto>> GetAllRecommendationAnswers();
        Task<AdminRecommendationAnswerDto?> GetRecommendationAnswerById(int id);
        Task<AdminRecommendationAnswerDto> CreateRecommendationAnswer(CreateRecommendationAnswerDto dto);
        Task<AdminRecommendationAnswerDto?> UpdateRecommendationAnswer(int id, UpdateRecommendationAnswerDto dto);
        Task<bool> DeleteRecommendationAnswer(int id);
    }
}
