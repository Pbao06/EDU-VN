using Source.DTOs.Admin;

namespace Source.Service.Admin.Interface
{
    public interface IAdminRecoQuestions
    {
        Task<List<AdminRecommendationQuestionDto>> GetAllRecommendationQuestions(); // get all 
        Task<AdminRecommendationQuestionDto?> GetRecommendationQuestionById(int id);// get specific
        Task<AdminRecommendationQuestionDto> CreateRecommendationQuestion(CreateRecommendationQuestionDto dto);//create
        Task<AdminRecommendationQuestionDto?> UpdateRecommendationQuestion(int id, UpdateRecommendationQuestionDto dto);//edit
        Task<bool> DeleteRecommendationQuestion(int id);
    }
}
