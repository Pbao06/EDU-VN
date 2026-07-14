using Source.DTOs.Admin;

namespace Source.Service.Admin.Interface
{
    public interface IAdminAnswerCareerWeightService
    {
        Task<List<AdminAnswerCareerWeightDto>> GetAllAnswerCareerWeights();
        Task<AdminAnswerCareerWeightDto?> GetAnswerCareerWeightById(int id);
        Task<AdminAnswerCareerWeightDto> CreateAnswerCareerWeight(CreateAnswerCareerWeightDto dto);
        Task<AdminAnswerCareerWeightDto?> UpdateAnswerCareerWeight(int id, UpdateAnswerCareerWeightDto dto);
        Task<bool> DeleteAnswerCareerWeight(int id);
    }
}