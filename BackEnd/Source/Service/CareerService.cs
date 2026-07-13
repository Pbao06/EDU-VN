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
    }
}
