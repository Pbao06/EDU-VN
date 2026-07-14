using Source.Data;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;
using Source.Models;

namespace Source.Service.Admin
{
    public class AdminQuizService : IAdminQuizService
    {
        private readonly ApplicationDbContext _context;

        public AdminQuizService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Quiz CRUD
        public async Task<List<AdminQuizDto>> GetAllQuizzes()
        {
            return await _context.Quizzes
                .Include(q => q.Field)
                .Select(q => new AdminQuizDto
                {
                    Id = q.Id,
                    Code = q.Code,
                    Title = q.Title,
                    Description = q.Description,
                    FieldId = q.FieldId,
                    FieldName = q.Field != null ? q.Field.Name : null,
                    DurationMinutes = q.DurationMinutes
                })
                .ToListAsync();
        }

        public async Task<AdminQuizDto?> GetQuizById(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Field)
                .FirstOrDefaultAsync(q => q.Id == id);
            
            if (quiz == null) throw new NotFoundException(" Khong tim thay QUiz");

            return new AdminQuizDto
            {
                Id = quiz.Id,
                Code = quiz.Code,
                Title = quiz.Title,
                Description = quiz.Description,
                FieldId = quiz.FieldId,
                FieldName = quiz.Field != null ? quiz.Field.Name : null,
                DurationMinutes = quiz.DurationMinutes
            };
        }

        public async Task<AdminQuizDto> CreateQuiz(CreateQuizDto dto)
        {
            // Check if Code already exists
            if (await _context.Quizzes.AnyAsync(q => q.Code == dto.Code))
            {
                throw new BadRequestException($"Quiz with Code '{dto.Code}' already exists");
            }

            // Check if Field exists (if provided)
            string? fieldName = null;
            if (dto.FieldId.HasValue)
            {
                var field = await _context.Fields.FindAsync(dto.FieldId.Value);
                if (field == null)
                {
                    throw new NotFoundException($"Field with ID {dto.FieldId.Value} not found");
                }
                fieldName = field.Name;
            }

            var quiz = new Quiz
            {
                Code = dto.Code,
                Title = dto.Title,
                Description = dto.Description,
                FieldId = dto.FieldId,
                DurationMinutes = dto.DurationMinutes
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return new AdminQuizDto
            {
                Id = quiz.Id,
                Code = quiz.Code,
                Title = quiz.Title,
                Description = quiz.Description,
                FieldId = quiz.FieldId,
                FieldName = fieldName,
                DurationMinutes = quiz.DurationMinutes
            };
        }

        public async Task<AdminQuizDto?> UpdateQuiz(int id, UpdateQuizDto dto)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) throw new NotFoundException(" Khong tim thay Quiz");

            // Check if Code already exists for another quiz
            if (await _context.Quizzes.AnyAsync(q => q.Code == dto.Code && q.Id != id))
            {
                throw new BadRequestException($"Quiz with Code '{dto.Code}' already exists");
            }

            // Check if Field exists (if provided)
            string? fieldName = null;
            if (dto.FieldId.HasValue)
            {
                var field = await _context.Fields.FindAsync(dto.FieldId.Value);
                if (field == null)
                {
                    throw new NotFoundException($"Field with ID {dto.FieldId.Value} not found");
                }
                fieldName = field.Name;
            }

            quiz.Code = dto.Code;
            quiz.Title = dto.Title;
            quiz.Description = dto.Description;
            quiz.FieldId = dto.FieldId;
            quiz.DurationMinutes = dto.DurationMinutes;

            await _context.SaveChangesAsync();

            return new AdminQuizDto
            {
                Id = quiz.Id,
                Code = quiz.Code,
                Title = quiz.Title,
                Description = quiz.Description,
                FieldId = quiz.FieldId,
                FieldName = fieldName,
                DurationMinutes = quiz.DurationMinutes
            };
        }

        public async Task<bool> DeleteQuiz(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return false;

            // Check if quiz has related questions
            if (await _context.RecommendationQuestions.AnyAsync(rq => rq.QuizId == id))
            {
                throw new BadRequestException("Cannot delete quiz that has related questions");
            }

            // Check if quiz has related recommendations
            if (await _context.QuizCareerRecommendations.AnyAsync(qcr => qcr.QuizId == id))
            {
                throw new BadRequestException("Cannot delete quiz that has related recommendations");
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}