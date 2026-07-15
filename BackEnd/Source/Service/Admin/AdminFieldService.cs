using Source.Data;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Source.Service.Admin
{
    public class AdminFieldService : IAdminFieldService
    {
        private readonly ApplicationDbContext _context;

        public AdminFieldService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminFieldDto>> GetAllFields()
        {
            return await _context.Fields
                .Select(f => new AdminFieldDto
                {
                    Id = f.Id,
                    Code = f.Code,
                    Name = f.Name,
                    Description = f.Description
                })
                .ToListAsync();
        }

        public async Task<AdminFieldDto?> GetFieldById(int id)
        {
            var field = await _context.Fields.FindAsync(id);
            if (field == null) return null;

            return new AdminFieldDto
            {
                Id = field.Id,
                Code = field.Code,
                Name = field.Name,
                Description = field.Description
            };
        }

        public async Task<AdminFieldDto> CreateField(CreateFieldDto dto)
        {
            // Check if Code already exists
            if (await _context.Fields.AnyAsync(f => f.Code == dto.Code))
            {
                throw new BadRequestException($"Field with Code '{dto.Code}' already exists");
            }

            var field = new Models.Field
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Fields.Add(field);
            await _context.SaveChangesAsync();

            return new AdminFieldDto
            {
                Id = field.Id,
                Code = field.Code,
                Name = field.Name,
                Description = field.Description
            };
        }

        public async Task<AdminFieldDto?> UpdateField(int id, UpdateFieldDto dto)
        {
            var field = await _context.Fields.FindAsync(id);
            if (field == null)  throw new NotFoundException(" Khong tim thay field de edit ");

            // Check if Code already exists for another field
            if (await _context.Fields.AnyAsync(f => f.Code == dto.Code && f.Id != id))
            {
                throw new BadRequestException($"Field with Code '{dto.Code}' already exists");
            }

            field.Code = dto.Code;
            field.Name = dto.Name;
            field.Description = dto.Description;

            await _context.SaveChangesAsync();

            return new AdminFieldDto
            {
                Id = field.Id,
                Code = field.Code,
                Name = field.Name,
                Description = field.Description
            };
        }

        public async Task<bool> DeleteField(int id)
        {
            var field = await _context.Fields.FindAsync(id);
            if (field == null) return false;

            // Check if field has related careers
            if (await _context.Careers.AnyAsync(c => c.FieldId == id))
            {
                throw new BadRequestException("Cannot delete field that has related careers");
            }

            // Check if field has related quizzes
            if (await _context.Quizzes.AnyAsync(q => q.FieldId == id))
            {
                throw new BadRequestException("Cannot delete field that has related quizzes");
            }

            _context.Fields.Remove(field);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}