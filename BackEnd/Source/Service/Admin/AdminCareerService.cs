using Microsoft.EntityFrameworkCore;
using Source.Data;
using Source.DTOs;
using Source.DTOs.Admin;
using Source.Middleware;
using Source.Service.Admin.Interface;

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
            // materialize a simple projection first to avoid unsupported expression-tree calls
            var items = await _context.Careers
                .Include(c => c.Field)
                .Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.Name,
                    c.FieldId,
                    FieldName = c.Field.Name,
                    c.Description,
                    c.Responsibilities,
                    c.MinSalary,
                    c.MaxSalary,
                    c.DemandLevel,
                    c.IconUrl,
                    c.PopularityScore,
                    RequiredSkills = c.RequiredSkills,
                    Tags = c.Tags,
                    c.Difficulty
                })
                .ToListAsync();

            // perform string parsing on the client side
            return items.Select(c => new AdminCareerDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                FieldId = c.FieldId,
                FieldName = c.FieldName,
                Description = c.Description,
                Responsibilities = c.Responsibilities,
                MinSalary = c.MinSalary,
                MaxSalary = c.MaxSalary,
                DemandLevel = c.DemandLevel,
                IconUrl = c.IconUrl,
                PopularityScore = c.PopularityScore,
                RequiredSkills = !string.IsNullOrEmpty(c.RequiredSkills)
                    ? c.RequiredSkills.Split(',').Select(s => s.Trim()).ToList()
                    : new List<string>(),
                Tags = !string.IsNullOrEmpty(c.Tags)
                    ? c.Tags.Split(',').Select(s => s.Trim()).ToList()
                    : new List<string>(),
                Difficulty = c.Difficulty
            }).ToList();
        }

        //public async Task<AdminCareerDto?> GetCareerById(int id)
        //{
        //    var career = await _context.Careers
        //        .Include(c => c.Field)
        //        .FirstOrDefaultAsync(c => c.Id == id);
            
        //    if (career == null) return null;

        //    return new AdminCareerDto
        //    {
        //        Id = career.Id,
        //        Code = career.Code,
        //        Name = career.Name,
        //        FieldId = career.FieldId,
        //        FieldName = career.Field.Name,
        //        Description = career.Description,
        //        Responsibilities = career.Responsibilities,
        //        MinSalary = career.MinSalary,
        //        MaxSalary = career.MaxSalary,
        //        DemandLevel = career.DemandLevel,
        //        IconUrl = career.IconUrl,
        //        PopularityScore = career.PopularityScore
        //    };
        //}
        //GET DETAIL CAREER PUBLIC - lấy chi tiết career cho public display (không cần authentication)
        public async Task<CareerDetailDto> GetDetailCareerPublic(int id)
        {
            var career = await _context.Careers
                .Include(c => c.Field)
                .Include(c => c.CareerSubjects)
                .ThenInclude(cs => cs.Subject)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (career == null) throw new NotFoundException("Không tìm thấy nghề");

            // Parse RequiredSkills from string to list
            var requiredSkillsList = !string.IsNullOrEmpty(career.RequiredSkills)
                ? career.RequiredSkills.Split(',').Select(s => s.Trim()).ToList()
                : new List<string>();

            // Parse Tags from string to list
            var tagsList = !string.IsNullOrEmpty(career.Tags)
                ? career.Tags.Split(',').Select(s => s.Trim()).ToList()
                : new List<string>();

            // Get related subjects from CareerSubjects
            var relatedSubjectsList = career.CareerSubjects
                .OrderBy(cs => cs.Priority)
                .Select(cs => cs.Subject.Name)
                .ToList();

            var dto = new CareerDetailDto
            {
                Id = career.Id,
                Name = career.Name,
                Description = career.Description,
                DemandLevel = career.DemandLevel,
                IconUrl = career.IconUrl,
                MaxSalary = career.MaxSalary,
                MinSalary = career.MinSalary,
                Responsibilities = career.Responsibilities,
                Category = career.Field?.Name ?? "General",
                Difficulty = career.Difficulty,
                RequiredSkills = requiredSkillsList,
                Tags = tagsList,
                RelatedSubjects = relatedSubjectsList
            };
            return dto;
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
                PopularityScore = dto.PopularityScore,
                Difficulty = dto.Difficulty,
                // CHIỀU TẠO MỚI (DTO List -> Entity string): Dùng string.Join
                RequiredSkills = dto.RequiredSkills != null && dto.RequiredSkills.Any()
                ? string.Join(",", dto.RequiredSkills)
                : string.Empty,

                Tags = dto.Tags != null && dto.Tags.Any()
                ? string.Join(",", dto.Tags)
                : string.Empty,
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
                PopularityScore = career.PopularityScore,
                Difficulty = career.Difficulty,
                // CHIỀU TRẢ VỀ DTO (Entity string -> DTO List): Dùng Split
                RequiredSkills = !string.IsNullOrEmpty(career.RequiredSkills)
                ? career.RequiredSkills.Split(',').Select(s => s.Trim()).ToList()
                : new List<string>(),

                Tags = !string.IsNullOrEmpty(career.Tags)
                ? career.Tags.Split(',').Select(s => s.Trim()).ToList()
                : new List<string>()
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
            career.Difficulty=dto.Difficulty;
            // CHIỀU TẠO MỚI (DTO List -> Entity string): Dùng string.Join
            career.RequiredSkills = dto.RequiredSkills != null && dto.RequiredSkills.Any()
            ? string.Join(",", dto.RequiredSkills)
            : string.Empty;

            career.Tags = dto.Tags != null && dto.Tags.Any()
              ? string.Join(",", dto.Tags)
              : string.Empty;




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
                PopularityScore = career.PopularityScore,
                Difficulty = career.Difficulty,
                // CHIỀU TRẢ VỀ DTO (Entity string -> DTO List): Dùng Split
                RequiredSkills = !string.IsNullOrEmpty(career.RequiredSkills)
                ? career.RequiredSkills.Split(',').Select(s => s.Trim()).ToList()
                : new List<string>(),

                Tags = !string.IsNullOrEmpty(career.Tags)
                ? career.Tags.Split(',').Select(s => s.Trim()).ToList()
                : new List<string>()

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