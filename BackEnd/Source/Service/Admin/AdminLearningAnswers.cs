using Microsoft.EntityFrameworkCore;
using Source.Data;
using Source.DTOs.Admin;
using Source.Middleware;
using Source.Service.Admin.Interface;
using System.Security.AccessControl;

namespace Source.Service.Admin
{
    public class AdminLearningAnswers: IAdminLearningAnswers
    {
        private readonly ApplicationDbContext _context;
        public AdminLearningAnswers(ApplicationDbContext context)
        {
            _context = context;
        }
        // LearningAnswer CRUD
        public async Task<List<AdminLearningAnswerDto>> GetAllLearningAnswers()
        {
            return await _context.LearningAnswers
                .Include(la => la.LearningQuestion)
                .Select(la => new AdminLearningAnswerDto
                {
                    Id = la.Id,
                    Content = la.Content,
                    IsCorrect = la.IsCorrect,
                    Explanation = la.Explanation,
                    LearningQuestionId = la.LearningQuestionId,
                    QuestionContent = la.LearningQuestion.Content
                })
                .ToListAsync();
        }

        public async Task<AdminLearningAnswerDto?> GetLearningAnswerById(int id)
        {
            var answer = await _context.LearningAnswers
                .Include(la => la.LearningQuestion)
                .FirstOrDefaultAsync(la => la.Id == id);

            if (answer == null) return null;

            return new AdminLearningAnswerDto
            {
                Id = answer.Id,
                Content = answer.Content,
                IsCorrect = answer.IsCorrect,
                Explanation = answer.Explanation,
                LearningQuestionId = answer.LearningQuestionId,
                QuestionContent = answer.LearningQuestion.Content
            };
        }

        public async Task<AdminLearningAnswerDto> CreateLearningAnswer(CreateLearningAnswerDto dto)
        {
            // Check if Question exists
            var question = await _context.LearningQuestions.FindAsync(dto.LearningQuestionId);
            if (question == null)
            {
                throw new NotFoundException($"LearningQuestion with ID {dto.LearningQuestionId} not found");
            }

            var answer = new Models.LearningAnswer
            {
                Content = dto.Content,
                IsCorrect = dto.IsCorrect,
                Explanation = dto.Explanation,
                LearningQuestionId = dto.LearningQuestionId,
                AnswerIndex=dto.AnswerIndex
            };

            _context.LearningAnswers.Add(answer);
            await _context.SaveChangesAsync();

            return new AdminLearningAnswerDto
            {
                Id = answer.Id,
                Content = answer.Content,
                IsCorrect = answer.IsCorrect,
                Explanation = answer.Explanation,
                LearningQuestionId = answer.LearningQuestionId,
                QuestionContent = question.Content,
                AnswerIndex=answer.AnswerIndex?? 0
            };
        }

        public async Task<AdminLearningAnswerDto?> UpdateLearningAnswer(int id, UpdateLearningAnswerDto dto)
        {
            var answer = await _context.LearningAnswers.FindAsync(id);
            if (answer == null) return null;

            // Check if Question exists
            var question = await _context.LearningQuestions.FindAsync(dto.LearningQuestionId);
            if (question == null)
            {
                throw new NotFoundException($"LearningQuestion with ID {dto.LearningQuestionId} not found");
            }

            answer.Content = dto.Content;
            answer.IsCorrect = dto.IsCorrect;
            answer.Explanation = dto.Explanation;
            answer.LearningQuestionId = dto.LearningQuestionId;

            await _context.SaveChangesAsync();

            return new AdminLearningAnswerDto
            {
                Id = answer.Id,
                Content = answer.Content,
                IsCorrect = answer.IsCorrect,
                Explanation = answer.Explanation,
                LearningQuestionId = answer.LearningQuestionId,
                QuestionContent = question.Content
            };
        }

        public async Task<bool> DeleteLearningAnswer(int id)
        {
            var answer = await _context.LearningAnswers.FindAsync(id);
            if (answer == null) return false;

            // Check if answer has related user answers
            if (await _context.LearningUserAnswers.AnyAsync(lua => lua.LearningAnswerId == id))
            {
                throw new BadRequestException("Cannot delete answer that has related user answers");
            }

            _context.LearningAnswers.Remove(answer);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
