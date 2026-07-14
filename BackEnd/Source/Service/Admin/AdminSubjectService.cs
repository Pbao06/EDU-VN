using Source.Data;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Source.Service.Admin
{
    public class AdminSubjectService : IAdminSubjectService
    {
        private readonly ApplicationDbContext _context;

        public AdminSubjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminSubjectDto>> GetAllSubjects()
        {
            return await _context.Subjects
                .Select(s => new AdminSubjectDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Description = s.Description,
                    Type = s.Type
                })
                .ToListAsync();
        }

        public async Task<AdminSubjectDto?> GetSubjectById(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return null;

            return new AdminSubjectDto
            {
                Id = subject.Id,
                Code = subject.Code,
                Name = subject.Name,
                Description = subject.Description,
                Type = subject.Type
            };
        }

        public async Task<AdminSubjectDto> CreateSubject(CreateSubjectDto dto)
        {
            // Check if Code already exists
            if (await _context.Subjects.AnyAsync(s => s.Code == dto.Code))
            {
                throw new BadRequestException($"Subject with Code '{dto.Code}' already exists");
            }

            var subject = new Models.Subject
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                Type = dto.Type
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return new AdminSubjectDto
            {
                Id = subject.Id,
                Code = subject.Code,
                Name = subject.Name,
                Description = subject.Description,
                Type = subject.Type
            };
        }

        public async Task<AdminSubjectDto?> UpdateSubject(int id, UpdateSubjectDto dto)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return null;

            // Check if Code already exists for another subject
            if (await _context.Subjects.AnyAsync(s => s.Code == dto.Code && s.Id != id))
            {
                throw new BadRequestException($"Subject with Code '{dto.Code}' already exists");
            }

            subject.Code = dto.Code;
            subject.Name = dto.Name;
            subject.Description = dto.Description;
            subject.Type = dto.Type;

            await _context.SaveChangesAsync();

            return new AdminSubjectDto
            {
                Id = subject.Id,
                Code = subject.Code,
                Name = subject.Name,
                Description = subject.Description,
                Type = subject.Type
            };
        }

        public async Task<bool> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return false;

            // Check if subject has related topics
            if (await _context.Topics.AnyAsync(t => t.SubjectId == id))
            {
                throw new BadRequestException("Cannot delete subject that has related topics");
            }

            // Check if subject has related career subjects
            if (await _context.CareerSubjects.AnyAsync(cs => cs.SubjectId == id))
            {
                throw new BadRequestException("Cannot delete subject that has related career subjects");
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}