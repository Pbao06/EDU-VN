using Source.DTOs.Admin;

namespace Source.Service.Admin.Interface
{
    public interface IAdminFieldService
    {
        Task<List<AdminFieldDto>> GetAllFields();
        Task<AdminFieldDto?> GetFieldById(int id);
        Task<AdminFieldDto> CreateField(CreateFieldDto dto);
        Task<AdminFieldDto?> UpdateField(int id, UpdateFieldDto dto);
        Task<bool> DeleteField(int id);
    }
}