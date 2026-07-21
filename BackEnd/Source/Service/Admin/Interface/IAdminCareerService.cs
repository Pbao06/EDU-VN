using Source.DTOs.Admin;
using Source.DTOs;

namespace Source.Service.Admin.Interface
{
    public interface IAdminCareerService
    {
        Task<List<AdminCareerDto>> GetAllCareers();
        // Task<AdminCareerDto?> GetCareerById(int id);
        Task<CareerDetailDto> GetDetailCareerPublic(int id);
        Task<AdminCareerDto> CreateCareer(CreateCareerDto dto);
        Task<AdminCareerDto?> UpdateCareer(int id, UpdateCareerDto dto);
        Task<bool> DeleteCareer(int id);
    }
}