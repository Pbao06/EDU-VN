using Source.Data;
using Source.DTOs;
using Source.Service.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Source.Service
{
    public class TopicService : ITopicService
    {
        private readonly ApplicationDbContext _context;

        public TopicService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Lấy chi tiết topic với LearningQuestions
        /// </summary>
        public async Task<TopicDetailDto> GetTopicDetail(int topicId, string userId)
        {
            var topic = await _context.Topics
                .Include(t => t.Subject)
                .Include(t => t.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(t => t.Id == topicId);

            if (topic == null) throw new NotFoundException("Không tìm thấy Topic");

            // Lấy user answers cho questions trong topic này
            var userAnswers = await _context.LearningUserAnswers
                .Where(lua => lua.UserId == userId && lua.LearningQuestion.TopicId == topicId)
                .ToListAsync();

            var questionDtos = new List<LearningQuestionDto>();
            int completedQuestions = 0;

            foreach (var question in topic.Questions)
            {
                var userAnswer = userAnswers.FirstOrDefault(ua => ua.LearningQuestionId == question.Id);
                var isCompleted = userAnswer != null;
                if (isCompleted) completedQuestions++;

                var answerDtos = question.Answers.Select(a => new LearningAnswerDto
                {
                    Id = a.Id,
                    Content = a.Content,
                    IsCorrect = a.IsCorrect,
                    Explanation = a.Explanation,
                    LearningQuestionId = a.LearningQuestionId
                }).ToList();

                questionDtos.Add(new LearningQuestionDto
                {
                    Id = question.Id,
                    Content = question.Content,
                    Explanation = question.Explanation,
                    Hint = question.Hint,
                    Difficulty = question.Difficulty,
                    TopicId = question.TopicId,
                    Answers = answerDtos,
                    UserAnswerId = userAnswer?.LearningAnswerId,
                    IsUserCorrect = userAnswer?.IsCorrect
                });
            }

            var progress = await _context.UserProgresses
                .FirstOrDefaultAsync(up => up.UserId == userId && up.TopicId == topicId);

            int totalQuestions = topic.Questions.Count;
            double topicProgress = totalQuestions > 0 ? (double)completedQuestions / totalQuestions * 100 : 0;
            bool isTopicCompleted = progress != null && progress.CompletionPercentage >= 100;
            bool isInProgress = completedQuestions > 0 && !isTopicCompleted;

            return new TopicDetailDto
            {
                Id = topic.Id,
                Name = topic.Name,
                Description = topic.Description,
                DifficultyLevel = topic.DifficultyLevel,
                SubjectId = topic.SubjectId,
                SubjectName = topic.Subject.Name,
                TotalQuestions = totalQuestions,
                CompletedQuestions = completedQuestions,
                TopicProgress = topicProgress,
                IsCompleted = isTopicCompleted,
                IsInProgress = isInProgress,
                LastAccessedAt = progress?.LastAccessedAt,
                Questions = questionDtos
            };
        }

        /// <summary>
        /// Submit answers cho topic và cập nhật progress
        /// </summary>
        public async Task<SubmitTopicAnswersResponseDto> SubmitTopicAnswers(string userId, SubmitTopicAnswersDto request)
        {
            var topic = await _context.Topics
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.Id == request.TopicId);

            if (topic == null) throw new NotFoundException("Không tìm thấy Topic");

            var learningPath = await _context.UserLearningPaths
                .FirstOrDefaultAsync(lp => lp.Id == request.LearningPathId && lp.UserId == userId);

            if (learningPath == null) throw new NotFoundException("Không tìm thấy Learning Path");

            int correctAnswers = 0;
            int totalQuestions = request.Answers.Count;

            foreach (var answerPair in request.Answers)
            {
                var questionId = answerPair.Key;
                var answerId = answerPair.Value;

                // Lấy question và answer để kiểm tra
                var question = await _context.LearningQuestions
                    .Include(q => q.Answers)
                    .FirstOrDefaultAsync(q => q.Id == questionId);

                if (question == null) continue;

                var answer = question.Answers.FirstOrDefault(a => a.Id == answerId);
                if (answer == null) continue;

                bool isCorrect = answer.IsCorrect;
                if (isCorrect) correctAnswers++;

                // Kiểm tra user đã trả lời question này chưa
                var existingUserAnswer = await _context.LearningUserAnswers
                    .FirstOrDefaultAsync(lua => lua.UserId == userId && lua.LearningQuestionId == questionId);

                if (existingUserAnswer != null)
                {
                    // Update existing answer
                    existingUserAnswer.LearningAnswerId = answerId;
                    existingUserAnswer.IsCorrect = isCorrect;
                    existingUserAnswer.Score = isCorrect ? 1 : 0;
                    existingUserAnswer.AnsweredAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new user answer
                    var userAnswer = new Models.LearningUserAnswer
                    {
                        UserId = userId,
                        LearningQuestionId = questionId,
                        LearningAnswerId = answerId,
                        IsCorrect = isCorrect,
                        Score = isCorrect ? 1 : 0,
                        AnsweredAt = DateTime.UtcNow
                    };
                    _context.LearningUserAnswers.Add(userAnswer);
                }
            }

            await _context.SaveChangesAsync();

            // Cập nhật UserProgress
            var totalQuestionsInTopic = await _context.LearningQuestions
                .Where(lq => lq.TopicId == request.TopicId)
                .CountAsync();

            var totalCompletedQuestions = await _context.LearningUserAnswers
                .Where(lua => lua.UserId == userId && lua.LearningQuestion.TopicId == request.TopicId)
                .Select(lua => lua.LearningQuestionId)
                .Distinct()
                .CountAsync();

            var userProgress = await _context.UserProgresses
                .FirstOrDefaultAsync(up => up.UserId == userId && up.TopicId == request.TopicId);

            bool isTopicCompleted = totalCompletedQuestions >= totalQuestionsInTopic && totalQuestionsInTopic > 0;
            int completionPercentage = isTopicCompleted ? 100 : (int)((double)totalCompletedQuestions / totalQuestionsInTopic * 100);

            if (userProgress == null)
            {
                userProgress = new Models.UserProgress
                {
                    UserId = userId,
                    TopicId = request.TopicId,
                    CompletionPercentage = completionPercentage,
                    LastAccessedAt = DateTime.UtcNow
                };
                _context.UserProgresses.Add(userProgress);
            }
            else
            {
                userProgress.CompletionPercentage = completionPercentage;
                userProgress.LastAccessedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            double score = totalQuestions > 0 ? (double)correctAnswers / totalQuestions * 100 : 0;

            var topicProgressDto = await MapToTopicSummaryDto(topic, userId);
            
            // Cập nhật subject progress nếu topic hoàn thành
            SubjectSummaryDto? subjectProgressDto = null;
            if (isTopicCompleted)
            {
                subjectProgressDto = await MapToSubjectSummaryDto(topic.Subject, userId);
            }

            return new SubmitTopicAnswersResponseDto
            {
                Success = true,
                Message = isTopicCompleted 
                    ? "Topic đã hoàn thành!" 
                    : $"Đã nộp bài - Đúng {correctAnswers}/{totalQuestions} câu",
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctAnswers,
                Score = score,
                IsTopicCompleted = isTopicCompleted,
                TopicProgress = topicProgressDto,
                SubjectProgress = subjectProgressDto
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

        private async Task<SubjectSummaryDto> MapToSubjectSummaryDto(Models.Subject subject, string userId)
        {
            var careerSubject = await _context.CareerSubjects
                .Include(cs => cs.Career)
                .FirstOrDefaultAsync(cs => cs.SubjectId == subject.Id);

            int totalTopics = await _context.Topics
                .Where(t => t.SubjectId == subject.Id)
                .CountAsync();

            int completedTopics = 0;

            foreach (var topic in await _context.Topics.Where(t => t.SubjectId == subject.Id).ToListAsync())
            {
                var progress = await _context.UserProgresses
                    .FirstOrDefaultAsync(up => up.UserId == userId && up.TopicId == topic.Id);
                if (progress != null && progress.CompletionPercentage >= 100)
                {
                    completedTopics++;
                }
            }

            double subjectProgress = totalTopics > 0 ? (double)completedTopics / totalTopics * 100 : 0;
            bool isCompleted = completedTopics == totalTopics && totalTopics > 0;
            bool isInProgress = completedTopics > 0 && !isCompleted;

            return new SubjectSummaryDto
            {
                Id = subject.Id,
                Code = subject.Code,
                Name = subject.Name,
                Description = subject.Description,
                Type = subject.Type,
                Priority = careerSubject?.Priority ?? 0,
                Reason = careerSubject?.Reason ?? string.Empty,
                TotalTopics = totalTopics,
                CompletedTopics = completedTopics,
                SubjectProgress = subjectProgress,
                IsCompleted = isCompleted,
                IsInProgress = isInProgress
            };
        }

        #endregion
    }
}
