using Source.Data;
using Source.DTOs;
using Source.Service.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Source.Service
{
    public class CareerService: ICareerService
    {
        // DI 
        private readonly ApplicationDbContext _context;
        // constructor 
        public CareerService(ApplicationDbContext context)
        {
            _context = context;
        }
        // lasy danh sach careers cho user xem , 
        // va lay thong tin detail cho user xem de ma choose Career


        //GET LIST CAREER - lấy danh sách dựa trên lĩnh vực của user  chọn
        public async Task<List<ListCareerDto>> GetListCareer(string UserId)
        {
            // kiểm tra user tồn tại
             var user = await _context.Users.FindAsync(UserId);
            if (user == null) throw new NotFoundException("Không tìm thấy User");
            var dto = await _context.Careers
            .Where(x => x.FieldId == user.FieldId)
            .Select(x => new ListCareerDto
            {
                Id = x.Id,
                Name = x.Name,
                Salary = x.MaxSalary,
                DemandLevel = x.DemandLevel,
                IconUrl = x.IconUrl,
                ShortDescription = x.Description
            })
            .ToListAsync();
            if (dto.Count == 0) throw new NotFoundException("Không tìm thấy danh sách nghề");
            return dto;
        }
        // lấy full thông tin về nghành nghề đó
        public async Task<CareerDetailDto> GetDetailCareer(string userid,int id)
        {
            var user = await _context.Users.FindAsync(userid);
            if(user==null)
            {
                throw new NotFoundException("Không tìm thấy User");
            }
            var e= await _context.Careers.FirstOrDefaultAsync(c => c.FieldId==user.FieldId && c.Id==id);
            if (e == null) throw new NotFoundException("Không tìm thấy nghề");
             var dto = new CareerDetailDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                DemandLevel = e.DemandLevel,
                IconUrl = e.IconUrl,
                MaxSalary = e.MaxSalary,
                MinSalary = e.MinSalary,
                Responsibilities = e.Responsibilities,
            };
            return dto;
        }

        //GET ALL CAREERS PUBLIC - lấy tất cả careers cho public display (không cần authentication)
        public async Task<List<ListCareerDto>> GetAllCareersPublic()
        {
            var dto = await _context.Careers
            .Select(x => new ListCareerDto
            {
                Id = x.Id,
                Name = x.Name,
                Salary = x.MaxSalary,
                DemandLevel = x.DemandLevel,
                IconUrl = x.IconUrl,
                ShortDescription = x.Description
            })
            .ToListAsync();
            if (dto.Count == 0) throw new NotFoundException("Không tìm thấy danh sách nghề");
            return dto;
        }

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
    }
}
