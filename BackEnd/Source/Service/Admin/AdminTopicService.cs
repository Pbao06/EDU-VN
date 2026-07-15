using Source.Data;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Source.Service.Admin
{
    public class AdminTopicService : IAdminTopicService
    {
        private readonly ApplicationDbContext _context;

        public AdminTopicService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminTopicDto>> GetAllTopics()
        {
            return await _context.Topics
                .Include(t => t.Subject)
                .Select(t => new AdminTopicDto
                {
                    Id = t.Id,
                    SubjectId = t.SubjectId,
                    SubjectName = t.Subject.Name,
                    Name = t.Name,
                    Description = t.Description,
                    DifficultyLevel = t.DifficultyLevel
                })
                .ToListAsync();
        }

        public async Task<AdminTopicDto?> GetTopicById(int id)
        {
            var topic = await _context.Topics
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (topic == null) return null;

            return new AdminTopicDto
            {
                Id = topic.Id,
                SubjectId = topic.SubjectId,
                SubjectName = topic.Subject.Name,
                Name = topic.Name,
                Description = topic.Description,
                DifficultyLevel = topic.DifficultyLevel
            };
        }

        public async Task<AdminTopicDto> CreateTopic(CreateTopicDto dto)
        {
            // Check if Subject exists
            var subject = await _context.Subjects.FindAsync(dto.SubjectId);
            if (subject == null)
            {
                throw new NotFoundException($"Subject with ID {dto.SubjectId} not found");
            }

            var topic = new Models.Topic
            {
                SubjectId = dto.SubjectId,
                Name = dto.Name,
                Description = dto.Description,
                DifficultyLevel = dto.DifficultyLevel
            };

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            return new AdminTopicDto
            {
                Id = topic.Id,
                SubjectId = topic.SubjectId,
                SubjectName = subject.Name,
                Name = topic.Name,
                Description = topic.Description,
                DifficultyLevel = topic.DifficultyLevel
            };
        }

        public async Task<AdminTopicDto?> UpdateTopic(int id, UpdateTopicDto dto)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return null;

            // Check if Subject exists
            var subject = await _context.Subjects.FindAsync(dto.SubjectId);
            if (subject == null)
            {
                throw new NotFoundException($"Subject with ID {dto.SubjectId} not found");
            }

            topic.SubjectId = dto.SubjectId;
            topic.Name = dto.Name;
            topic.Description = dto.Description;
            topic.DifficultyLevel = dto.DifficultyLevel;

            await _context.SaveChangesAsync();

            return new AdminTopicDto
            {
                Id = topic.Id,
                SubjectId = topic.SubjectId,
                SubjectName = subject.Name,
                Name = topic.Name,
                Description = topic.Description,
                DifficultyLevel = topic.DifficultyLevel
            };
        }

        public async Task<bool> DeleteTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return false;

            // Check if topic has related learning questions
            if (await _context.LearningQuestions.AnyAsync(lq => lq.TopicId == id))
            {
                throw new BadRequestException("Cannot delete topic that has related learning questions");
            }

            // Check if topic has related user progress
            if (await _context.UserProgresses.AnyAsync(up => up.TopicId == id))
            {
                throw new BadRequestException("Cannot delete topic that has related user progress");
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}