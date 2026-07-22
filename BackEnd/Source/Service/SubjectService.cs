using Source.Data;
using Source.DTOs;
using Source.Service.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Source.Service
{
    public class SubjectService : ISubjectService
    {
        private readonly ApplicationDbContext _context;
        public SubjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy chi tiết subject với Topics list (không có Questions)
        /// </summary>
        public async Task<SubjectDetailDto> GetSubjectDetail(int subjectId, string userId)
        {
             // lay subject doc lap , get join lay danh sach topic 
            
 
            var subject = await _context.Subjects.Include(s=> s.Topics).FirstOrDefaultAsync(s=> s.Id==subjectId);
            var careersubject = await _context.CareerSubjects.FirstOrDefaultAsync(s => s.SubjectId == subject.Id);
            if (careersubject == null) throw new NotFoundException(" Not found Subject of Career ");
            if (subject == null) throw new NotFoundException(" Not found Subject ");

            //var topics= subject.Topics.Select(t => new TopicSummaryDto
            //{
            //    Id= t.Id,
            //    Name= t.Name,
            //    Description= t.Description,...
            //})

            var topicSummaryDtos = new List<TopicSummaryDto>();
            int completedTopics = 0;

            foreach (var topic in subject.Topics)
            {
                var topicDto = await MapToTopicSummaryDto(topic, userId);
                topicSummaryDtos.Add(topicDto);
                if (topicDto.IsCompleted)
                {
                    completedTopics++;
                }
            }

            int totalTopics = subject.Topics.Count;
            double subjectProgress = totalTopics > 0 ? (double)completedTopics / totalTopics * 100 : 0;
            bool isCompleted = completedTopics == totalTopics && totalTopics > 0;
            bool isInProgress = completedTopics > 0 && !isCompleted;

            return new SubjectDetailDto
            {
                Id = subject.Id,
                Code = subject.Code,
                Name = subject.Name,
                Description = subject.Description,
                Type = subject.Type,
                Priority = careersubject?.Priority ?? 0,
                Reason = careersubject?.Reason ?? string.Empty,
                TotalTopics = totalTopics,
                CompletedTopics = completedTopics,
                SubjectProgress = subjectProgress,
                IsCompleted = isCompleted,
                IsInProgress = isInProgress,
                Topics = topicSummaryDtos
            };
        }
        #region Helper Methods



        private async Task<TopicSummaryDto> MapToTopicSummaryDto(Models.Topic topic, string userId)
        {
            var progress = await _context.UserProgresses
                .FirstOrDefaultAsync(up => up.UserId == userId && up.TopicId == topic.Id);

            var totalQuestions = await _context.LearningQuestions
                .Where(lq => lq.TopicId == topic.Id)
                .CountAsync();

            var completedQuestions = await _context.LearningUserAnswers
                .Where(lua => lua.UserId == userId && lua.LearningQuestion.TopicId == topic.Id)
                .Select(lua => lua.LearningQuestionId)
                .Distinct()
                .CountAsync();

            double topicProgress = totalQuestions > 0 ? (double)completedQuestions / totalQuestions * 100 : 0;
            bool isCompleted = progress != null && progress.CompletionPercentage >= 100;
            bool isInProgress = completedQuestions > 0 && !isCompleted;

            return new TopicSummaryDto
            {
                Id = topic.Id,
                Name = topic.Name,
                Description = topic.Description,
                DifficultyLevel = topic.DifficultyLevel,
                TotalQuestions = totalQuestions,
                CompletedQuestions = completedQuestions,
                TopicProgress = topicProgress,
                IsCompleted = isCompleted,
                IsInProgress = isInProgress,
                LastAccessedAt = progress?.LastAccessedAt
            };
        }

        #endregion
    }
}
