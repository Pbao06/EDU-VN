using Source.Data;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Source.Service.Admin
{
    public class AdminCareerSubjectService : IAdminCareerSubjectService
    {
        private readonly ApplicationDbContext _context;

        public AdminCareerSubjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminCareerSubjectDto>> GetAllCareerSubjects()
        {
            return await _context.CareerSubjects
                .Include(cs => cs.Career)
                .Include(cs => cs.Subject)
                .Select(cs => new AdminCareerSubjectDto
                {
                    CareerId = cs.CareerId,
                    CareerName = cs.Career.Name,
                    SubjectId = cs.SubjectId,
                    SubjectName = cs.Subject.Name,
                    Priority = cs.Priority,
                    Reason = cs.Reason
                })
                .ToListAsync();
        }

        public async Task<AdminCareerSubjectDto?> GetCareerSubjectById(int careerId, int subjectId)
        {
            var careerSubject = await _context.CareerSubjects
                .Include(cs => cs.Career)
                .Include(cs => cs.Subject)
                .FirstOrDefaultAsync(cs => cs.CareerId == careerId && cs.SubjectId == subjectId);
            
            if (careerSubject == null) return null;

            return new AdminCareerSubjectDto
            {
                CareerId = careerSubject.CareerId,
                CareerName = careerSubject.Career.Name,
                SubjectId = careerSubject.SubjectId,
                SubjectName = careerSubject.Subject.Name,
                Priority = careerSubject.Priority,
                Reason = careerSubject.Reason
            };
        }

        public async Task<AdminCareerSubjectDto> CreateCareerSubject(CreateCareerSubjectDto dto)
        {
            // Check if Career exists
            var career = await _context.Careers.FindAsync(dto.CareerId);
            if (career == null)
            {
                throw new NotFoundException($"Career with ID {dto.CareerId} not found");
            }

            // Check if Subject exists
            var subject = await _context.Subjects.FindAsync(dto.SubjectId);
            if (subject == null)
            {
                throw new NotFoundException($"Subject with ID {dto.SubjectId} not found");
            }

            // Check if combination already exists
            if (await _context.CareerSubjects.AnyAsync(cs => 
                cs.CareerId == dto.CareerId && cs.SubjectId == dto.SubjectId))
            {
                throw new BadRequestException($"Career-Subject combination already exists");
            }

            var careerSubject = new Models.CareerSubject
            {
                CareerId = dto.CareerId,
                SubjectId = dto.SubjectId,
                Priority = dto.Priority,
                Reason = dto.Reason
            };

            _context.CareerSubjects.Add(careerSubject);
            await _context.SaveChangesAsync();

            return new AdminCareerSubjectDto
            {
                CareerId = careerSubject.CareerId,
                CareerName = career.Name,
                SubjectId = careerSubject.SubjectId,
                SubjectName = subject.Name,
                Priority = careerSubject.Priority,
                Reason = careerSubject.Reason
            };
        }

        public async Task<AdminCareerSubjectDto?> UpdateCareerSubject(int careerId, int subjectId, UpdateCareerSubjectDto dto)
        {
            var careerSubject = await _context.CareerSubjects
                .FirstOrDefaultAsync(cs => cs.CareerId == careerId && cs.SubjectId == subjectId);
            
            if (careerSubject == null) return null;

            // Check if Career exists
            var career = await _context.Careers.FindAsync(dto.CareerId);
            if (career == null)
            {
                throw new NotFoundException($"Career with ID {dto.CareerId} not found");
            }

            // Check if Subject exists
            var subject = await _context.Subjects.FindAsync(dto.SubjectId);
            if (subject == null)
            {
                throw new NotFoundException($"Subject with ID {dto.SubjectId} not found");
            }

            // Check if new combination already exists for another record
            if (await _context.CareerSubjects.AnyAsync(cs => 
                cs.CareerId == dto.CareerId && 
                cs.SubjectId == dto.SubjectId && 
                (cs.CareerId != careerId || cs.SubjectId != subjectId)))
            {
                throw new BadRequestException($"Career-Subject combination already exists");
            }

            careerSubject.CareerId = dto.CareerId;
            careerSubject.SubjectId = dto.SubjectId;
            careerSubject.Priority = dto.Priority;
            careerSubject.Reason = dto.Reason;

            await _context.SaveChangesAsync();

            return new AdminCareerSubjectDto
            {
                CareerId = careerSubject.CareerId,
                CareerName = career.Name,
                SubjectId = careerSubject.SubjectId,
                SubjectName = subject.Name,
                Priority = careerSubject.Priority,
                Reason = careerSubject.Reason
            };
        }

        public async Task<bool> DeleteCareerSubject(int careerId, int subjectId)
        {
            var careerSubject = await _context.CareerSubjects
                .FirstOrDefaultAsync(cs => cs.CareerId == careerId && cs.SubjectId == subjectId);
            
            if (careerSubject == null) return false;

            _context.CareerSubjects.Remove(careerSubject);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}