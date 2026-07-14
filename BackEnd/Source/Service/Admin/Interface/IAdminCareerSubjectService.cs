using Source.DTOs.Admin;

namespace Source.Service.Admin.Interface
{
    public interface IAdminCareerSubjectService
    {
        Task<List<AdminCareerSubjectDto>> GetAllCareerSubjects();
        Task<AdminCareerSubjectDto?> GetCareerSubjectById(int careerId, int subjectId);
        Task<AdminCareerSubjectDto> CreateCareerSubject(CreateCareerSubjectDto dto);
        Task<AdminCareerSubjectDto?> UpdateCareerSubject(int careerId, int subjectId, UpdateCareerSubjectDto dto);
        Task<bool> DeleteCareerSubject(int careerId, int subjectId);
    }
}