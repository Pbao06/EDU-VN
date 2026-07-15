using Source.Data;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Source.Service.Admin
{
    public class AdminAnswerCareerWeightService : IAdminAnswerCareerWeightService
    {
        private readonly ApplicationDbContext _context;

        public AdminAnswerCareerWeightService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminAnswerCareerWeightDto>> GetAllAnswerCareerWeights()
        {
            return await _context.AnswerCareerWeights
                .Include(acw => acw.RecommendationAnswer)
                .Include(acw => acw.Career)
                .Select(acw => new AdminAnswerCareerWeightDto
                {
                    Id = acw.Id,
                    RecommendationAnswerId = acw.RecommendationAnswerId,
                    AnswerContent = acw.RecommendationAnswer.Content,
                    CareerId = acw.CareerId,
                    CareerName = acw.Career.Name,
                    Weight = acw.Weight
                })
                .ToListAsync();
        }

        public async Task<AdminAnswerCareerWeightDto?> GetAnswerCareerWeightById(int id)
        {
            var weight = await _context.AnswerCareerWeights
                .Include(acw => acw.RecommendationAnswer)
                .Include(acw => acw.Career)
                .FirstOrDefaultAsync(acw => acw.Id == id);
            
            if (weight == null) return null;

            return new AdminAnswerCareerWeightDto
            {
                Id = weight.Id,
                RecommendationAnswerId = weight.RecommendationAnswerId,
                AnswerContent = weight.RecommendationAnswer.Content,
                CareerId = weight.CareerId,
                CareerName = weight.Career.Name,
                Weight = weight.Weight
            };
        }

        public async Task<AdminAnswerCareerWeightDto> CreateAnswerCareerWeight(CreateAnswerCareerWeightDto dto)
        {
            // Check if RecommendationAnswer exists
            var answer = await _context.RecommendationAnswers.FindAsync(dto.RecommendationAnswerId);
            if (answer == null)
            {
                throw new NotFoundException($"RecommendationAnswer with ID {dto.RecommendationAnswerId} not found");
            }

            // Check if Career exists
            var career = await _context.Careers.FindAsync(dto.CareerId);
            if (career == null)
            {
                throw new NotFoundException($"Career with ID {dto.CareerId} not found");
            }

            // Check if combination already exists
            if (await _context.AnswerCareerWeights.AnyAsync(acw => 
                acw.RecommendationAnswerId == dto.RecommendationAnswerId && acw.CareerId == dto.CareerId))
            {
                throw new BadRequestException($"Weight for this Answer-Career combination already exists");
            }

            // Validate weight range
            if (dto.Weight < -3 || dto.Weight > 5)
            {
                throw new BadRequestException("Weight must be between -3 and 5");
            }

            var weight = new Models.AnswerCareerWeight
            {
                RecommendationAnswerId = dto.RecommendationAnswerId,
                CareerId = dto.CareerId,
                Weight = dto.Weight
            };

            _context.AnswerCareerWeights.Add(weight);
            await _context.SaveChangesAsync();

            return new AdminAnswerCareerWeightDto
            {
                Id = weight.Id,
                RecommendationAnswerId = weight.RecommendationAnswerId,
                AnswerContent = answer.Content,
                CareerId = weight.CareerId,
                CareerName = career.Name,
                Weight = weight.Weight
            };
        }

        public async Task<AdminAnswerCareerWeightDto?> UpdateAnswerCareerWeight(int id, UpdateAnswerCareerWeightDto dto)
        {
            var weight = await _context.AnswerCareerWeights.FindAsync(id);
            if (weight == null) return null;

            // Check if RecommendationAnswer exists
            var answer = await _context.RecommendationAnswers.FindAsync(dto.RecommendationAnswerId);
            if (answer == null)
            {
                throw new NotFoundException($"RecommendationAnswer with ID {dto.RecommendationAnswerId} not found");
            }

            // Check if Career exists
            var career = await _context.Careers.FindAsync(dto.CareerId);
            if (career == null)
            {
                throw new NotFoundException($"Career with ID {dto.CareerId} not found");
            }

            // Check if combination already exists for another record
            if (await _context.AnswerCareerWeights.AnyAsync(acw => 
                acw.RecommendationAnswerId == dto.RecommendationAnswerId && 
                acw.CareerId == dto.CareerId && 
                acw.Id != id))
            {
                throw new BadRequestException($"Weight for this Answer-Career combination already exists");
            }

            // Validate weight range
            if (dto.Weight < -3 || dto.Weight > 5)
            {
                throw new BadRequestException("Weight must be between -3 and 5");
            }

            weight.RecommendationAnswerId = dto.RecommendationAnswerId;
            weight.CareerId = dto.CareerId;
            weight.Weight = dto.Weight;

            await _context.SaveChangesAsync();

            return new AdminAnswerCareerWeightDto
            {
                Id = weight.Id,
                RecommendationAnswerId = weight.RecommendationAnswerId,
                AnswerContent = answer.Content,
                CareerId = weight.CareerId,
                CareerName = career.Name,
                Weight = weight.Weight
            };
        }

        public async Task<bool> DeleteAnswerCareerWeight(int id)
        {
            var weight = await _context.AnswerCareerWeights.FindAsync(id);
            if (weight == null) return false;

            _context.AnswerCareerWeights.Remove(weight);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}