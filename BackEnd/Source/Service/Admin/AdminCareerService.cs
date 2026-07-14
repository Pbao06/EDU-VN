using Source.Data;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Source.Service.Admin
{
    public class AdminCareerService : IAdminCareerService
    {
        private readonly ApplicationDbContext _context;

        public AdminCareerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminCareerDto>> GetAllCareers()
        {
            return await _context.Careers
                .Include(c => c.Field)
                .Select(c => new AdminCareerDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    FieldId = c.FieldId,
                    FieldName = c.Field.Name,
                    Description = c.Description,
                    Responsibilities = c.Responsibilities,
                    MinSalary = c.MinSalary,
                    MaxSalary = c.MaxSalary,
                    DemandLevel = c.DemandLevel,
                    IconUrl = c.IconUrl,
                    PopularityScore = c.PopularityScore
                })
                .ToListAsync();
        }

        public async Task<AdminCareerDto?> GetCareerById(int id)
        {
            var career = await _context.Careers
                .Include(c => c.Field)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (career == null) return null;

            return new AdminCareerDto
            {
                Id = career.Id,
                Code = career.Code,
                Name = career.Name,
                FieldId = career.FieldId,
                FieldName = career.Field.Name,
                Description = career.Description,
                Responsibilities = career.Responsibilities,
                MinSalary = career.MinSalary,
                MaxSalary = career.MaxSalary,
                DemandLevel = career.DemandLevel,
                IconUrl = career.IconUrl,
                PopularityScore = career.PopularityScore
            };
        }

        public async Task<AdminCareerDto> CreateCareer(CreateCareerDto dto)
        {
            // Check if Code already exists
            if (await _context.Careers.AnyAsync(c => c.Code == dto.Code))
            {
                throw new BadRequestException($"Career with Code '{dto.Code}' already exists");
            }

            // Check if Field exists
            var field = await _context.Fields.FindAsync(dto.FieldId);
            if (field == null)
            {
                throw new NotFoundException($"Field with ID {dto.FieldId} not found");
            }

            var career = new Models.Career
            {
                Code = dto.Code,
                Name = dto.Name,
                FieldId = dto.FieldId,
                Description = dto.Description,
                Responsibilities = dto.Responsibilities,
                MinSalary = dto.MinSalary,
                MaxSalary = dto.MaxSalary,
                DemandLevel = dto.DemandLevel,
                IconUrl = dto.IconUrl,
                PopularityScore = dto.PopularityScore
            };

            _context.Careers.Add(career);
            await _context.SaveChangesAsync();

            return new AdminCareerDto
            {
                Id = career.Id,
                Code = career.Code,
                Name = career.Name,
                FieldId = career.FieldId,
                FieldName = field.Name,
                Description = career.Description,
                Responsibilities = career.Responsibilities,
                MinSalary = career.MinSalary,
                MaxSalary = career.MaxSalary,
                DemandLevel = career.DemandLevel,
                IconUrl = career.IconUrl,
                PopularityScore = career.PopularityScore
            };
        }

        public async Task<AdminCareerDto?> UpdateCareer(int id, UpdateCareerDto dto)
        {
            var career = await _context.Careers.FindAsync(id);
            if (career == null) return null;

            // Check if Code already exists for another career
            if (await _context.Careers.AnyAsync(c => c.Code == dto.Code && c.Id != id))
            {
                throw new BadRequestException($"Career with Code '{dto.Code}' already exists");
            }

            // Check if Field exists
            var field = await _context.Fields.FindAsync(dto.FieldId);
            if (field == null)
            {
                throw new NotFoundException($"Field with ID {dto.FieldId} not found");
            }

            career.Code = dto.Code;
            career.Name = dto.Name;
            career.FieldId = dto.FieldId;
            career.Description = dto.Description;
            career.Responsibilities = dto.Responsibilities;
            career.MinSalary = dto.MinSalary;
            career.MaxSalary = dto.MaxSalary;
            career.DemandLevel = dto.DemandLevel;
            career.IconUrl = dto.IconUrl;
            career.PopularityScore = dto.PopularityScore;

            await _context.SaveChangesAsync();

            return new AdminCareerDto
            {
                Id = career.Id,
                Code = career.Code,
                Name = career.Name,
                FieldId = career.FieldId,
                FieldName = field.Name,
                Description = career.Description,
                Responsibilities = career.Responsibilities,
                MinSalary = career.MinSalary,
                MaxSalary = career.MaxSalary,
                DemandLevel = career.DemandLevel,
                IconUrl = career.IconUrl,
                PopularityScore = career.PopularityScore
            };
        }

        public async Task<bool> DeleteCareer(int id)
        {
            var career = await _context.Careers.FindAsync(id);
            if (career == null) return false;

            // Check if career has related career subjects
            if (await _context.CareerSubjects.AnyAsync(cs => cs.CareerId == id))
            {
                throw new BadRequestException("Cannot delete career that has related subjects");
            }

            // Check if career has related learning paths
            if (await _context.UserLearningPaths.AnyAsync(lp => lp.CareerId == id))
            {
                throw new BadRequestException("Cannot delete career that has related learning paths");
            }

            // Check if career has related recommendations
            if (await _context.QuizCareerRecommendations.AnyAsync(qcr => qcr.CareerId == id))
            {
                throw new BadRequestException("Cannot delete career that has related recommendations");
            }

            _context.Careers.Remove(career);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}