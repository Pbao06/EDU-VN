using Source.Data;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Source.Service.Admin
{
    public class AdminLearningQuestionService : IAdminLearningQuestionService
    {
        private readonly ApplicationDbContext _context;

        public AdminLearningQuestionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // LearningQuestion CRUD
        public async Task<List<AdminLearningQuestionDto>> GetAllLearningQuestions()
        {
            return await _context.LearningQuestions
                .Include(lq => lq.Topic)
                .Select(lq => new AdminLearningQuestionDto
                {
                    Id = lq.Id,
                    Content = lq.Content,
                    Explanation = lq.Explanation,
                    Hint = lq.Hint,
                    Difficulty = lq.Difficulty,
                    TopicId = lq.TopicId,
                    TopicName = lq.Topic.Name
                })
                .ToListAsync();
        }

        public async Task<AdminLearningQuestionDto?> GetLearningQuestionById(int id)
        {
            var question = await _context.LearningQuestions
                .Include(lq => lq.Topic)
                .FirstOrDefaultAsync(lq => lq.Id == id);
            
            if (question == null) return null;

            return new AdminLearningQuestionDto
            {
                Id = question.Id,
                Content = question.Content,
                Explanation = question.Explanation,
                Hint = question.Hint,
                Difficulty = question.Difficulty,
                TopicId = question.TopicId,
                TopicName = question.Topic.Name
            };
        }

        public async Task<AdminLearningQuestionDto> CreateLearningQuestion(CreateLearningQuestionDto dto)
        {
            // Check if Topic exists
            var topic = await _context.Topics.FindAsync(dto.TopicId);
            if (topic == null)
            {
                throw new NotFoundException($"Topic with ID {dto.TopicId} not found");
            }

            var question = new Models.LearningQuestion
            {
                Content = dto.Content,
                Explanation = dto.Explanation,
                Hint = dto.Hint,
                Difficulty = dto.Difficulty,
                TopicId = dto.TopicId
            };

            _context.LearningQuestions.Add(question);
            await _context.SaveChangesAsync();

            return new AdminLearningQuestionDto
            {
                Id = question.Id,
                Content = question.Content,
                Explanation = question.Explanation,
                Hint = question.Hint,
                Difficulty = question.Difficulty,
                TopicId = question.TopicId,
                TopicName = topic.Name
            };
        }

        public async Task<AdminLearningQuestionDto?> UpdateLearningQuestion(int id, UpdateLearningQuestionDto dto)
        {
            var question = await _context.LearningQuestions.FindAsync(id);
            if (question == null) return null;

            // Check if Topic exists
            var topic = await _context.Topics.FindAsync(dto.TopicId);
            if (topic == null)
            {
                throw new NotFoundException($"Topic with ID {dto.TopicId} not found");
            }

            question.Content = dto.Content;
            question.Explanation = dto.Explanation;
            question.Hint = dto.Hint;
            question.Difficulty = dto.Difficulty;
            question.TopicId = dto.TopicId;

            await _context.SaveChangesAsync();

            return new AdminLearningQuestionDto
            {
                Id = question.Id,
                Content = question.Content,
                Explanation = question.Explanation,
                Hint = question.Hint,
                Difficulty = question.Difficulty,
                TopicId = question.TopicId,
                TopicName = topic.Name
            };
        }

        public async Task<bool> DeleteLearningQuestion(int id)
        {
            var question = await _context.LearningQuestions.FindAsync(id);
            if (question == null) return false;

            // Check if question has related answers
            if (await _context.LearningAnswers.AnyAsync(la => la.LearningQuestionId == id))
            {
                throw new BadRequestException("Cannot delete question that has related answers");
            }

            // Check if question has related user answers
            if (await _context.LearningUserAnswers.AnyAsync(lua => lua.LearningQuestionId == id))
            {
                throw new BadRequestException("Cannot delete question that has related user answers");
            }

            _context.LearningQuestions.Remove(question);
            await _context.SaveChangesAsync();

            return true;
        }

       
    }
}