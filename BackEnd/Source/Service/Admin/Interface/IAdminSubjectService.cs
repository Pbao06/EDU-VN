using Source.DTOs.Admin;

namespace Source.Service.Admin.Interface
{
    public interface IAdminSubjectService
    {
        Task<List<AdminSubjectDto>> GetAllSubjects();
        Task<AdminSubjectDto?> GetSubjectById(int id);
        Task<AdminSubjectDto> CreateSubject(CreateSubjectDto dto);
        Task<AdminSubjectDto?> UpdateSubject(int id, UpdateSubjectDto dto);
        Task<bool> DeleteSubject(int id);
    }
}