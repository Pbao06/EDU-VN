using Microsoft.EntityFrameworkCore;
using Source.Data;
using Source.DTOs.Admin;
using Source.Middleware;
using Source.Service.Admin.Interface;

namespace Source.Service.Admin
{
    public class AdminRecommendationAnswer : IAdminRecoAnswers
    {
        private readonly ApplicationDbContext _context;
        public AdminRecommendationAnswer(ApplicationDbContext context)
        {
            _context = context;
        }
        // RecommendationAnswer CRUD
        public async Task<List<AdminRecommendationAnswerDto>> GetAllRecommendationAnswers()
        {
            return await _context.RecommendationAnswers
                .Include(ra => ra.RecommendationQuestion)
                .Select(ra => new AdminRecommendationAnswerDto
                {
                    Id = ra.Id,
                    Content = ra.Content,
                    RecommendationQuestionId = ra.RecommendationQuestionId,
                    QuestionContent = ra.RecommendationQuestion.Content
                })
                .ToListAsync();
        }

        public async Task<AdminRecommendationAnswerDto?> GetRecommendationAnswerById(int id)
        {
            var answer = await _context.RecommendationAnswers
                .Include(ra => ra.RecommendationQuestion)
                .FirstOrDefaultAsync(ra => ra.Id == id);

            if (answer == null) return null;

            return new AdminRecommendationAnswerDto
            {
                Id = answer.Id,
                Content = answer.Content,
                RecommendationQuestionId = answer.RecommendationQuestionId,
                QuestionContent = answer.RecommendationQuestion.Content
            };
        }

        public async Task<AdminRecommendationAnswerDto> CreateRecommendationAnswer(CreateRecommendationAnswerDto dto)
        {
            // Check if Question exists
            var question = await _context.RecommendationQuestions.FindAsync(dto.RecommendationQuestionId);
            if (question == null)
            {
                throw new NotFoundException($"RecommendationQuestion with ID {dto.RecommendationQuestionId} not found");
            }

            var answer = new Models.RecommendationAnswer
            {
                Content = dto.Content,
                RecommendationQuestionId = dto.RecommendationQuestionId
            };

            _context.RecommendationAnswers.Add(answer);
            await _context.SaveChangesAsync();

            return new AdminRecommendationAnswerDto
            {
                Id = answer.Id,
                Content = answer.Content,
                RecommendationQuestionId = answer.RecommendationQuestionId,
                QuestionContent = question.Content
            };
        }

        public async Task<AdminRecommendationAnswerDto?> UpdateRecommendationAnswer(int id, UpdateRecommendationAnswerDto dto)
        {
            var answer = await _context.RecommendationAnswers.FindAsync(id);
            if (answer == null) return null;

            // Check if Question exists
            var question = await _context.RecommendationQuestions.FindAsync(dto.RecommendationQuestionId);
            if (question == null)
            {
                throw new NotFoundException($"RecommendationQuestion with ID {dto.RecommendationQuestionId} not found");
            }

            answer.Content = dto.Content;
            answer.RecommendationQuestionId = dto.RecommendationQuestionId;

            await _context.SaveChangesAsync();

            return new AdminRecommendationAnswerDto
            {
                Id = answer.Id,
                Content = answer.Content,
                RecommendationQuestionId = answer.RecommendationQuestionId,
                QuestionContent = question.Content
            };
        }

        public async Task<bool> DeleteRecommendationAnswer(int id)
        {
            var answer = await _context.RecommendationAnswers.FindAsync(id);
            if (answer == null) return false;

            // Check if answer has related career weights
            if (await _context.AnswerCareerWeights.AnyAsync(acw => acw.RecommendationAnswerId == id))
            {
                throw new BadRequestException("Cannot delete answer that has related career weights");
            }

            // Check if answer has related user answers
            if (await _context.RecommendationUserAnswers.AnyAsync(rua => rua.RecommendationAnswerId == id))
            {
                throw new BadRequestException("Cannot delete answer that has related user answers");
            }

            _context.RecommendationAnswers.Remove(answer);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
