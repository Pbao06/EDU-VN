using Source.Data;
using Source.DTOs;
using Source.Service.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;
using Source.Models;
using System.Data;

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

            // var learningPath = await _context.UserLearningPaths
            //     .FirstOrDefaultAsync(lp => lp.Id == request.LearningPathId && lp.UserId == userId);
            // if (learningPath == null) throw new NotFoundException("Không tìm thấy Learning Path");


            // 1. Lấy TOÀN BỘ câu hỏi + đáp án thật của topic — nguồn sự thật duy nhất
             var questions = await _context.LearningQuestions
                .Where(q => q.TopicId == request.TopicId)
                .Include(q => q.Answers)
                .ToListAsync();
            

            int totalQuestions = questions.Count;
                // 2. Lấy sẵn các UserAnswer hiện có để update thay vì query từng cái trong loop
                ///BƯỚC 2: Lấy bài học sinh đã nộp trước đó (nếu có) ra so sánh
                /// -> "existingAnswers" = học sinh đã làm bài này lần nào chưa?

             var existingAnswers = await _context.LearningUserAnswers
                .Where(lua => lua.UserId == userId && lua.LearningQuestion.TopicId == request.TopicId)
                .ToListAsync();
                var existingByQuestionId = existingAnswers.ToDictionary(a => a.LearningQuestionId);
                int correctAnswers=0;
                //BƯỚC 3: Giáo viên cầm từng câu hỏi trong đề gốc, đi dò trong bài học sinh nộp
                // for mỗi câu hỏi trong đề gốc (questions):
            foreach(var question in questions) // lấy danh sácsh câu hỏi trong topic đã đc lấy 
            {
                // chỉ lấy question mà usse đã có trả lời và thuộc topic 
                if(!request.Answers.TryGetValue(question.Id,out var answerId)) continue; // user không trả lời câu này -> bỏ qua, vẫn tính vào totalQuestions
                var answer= question.Answers.FirstOrDefault(a=>a.Id==answerId); // dò tìm xem trong 1 câu thi các đáp án có sẵn của topic đã có đásp án mà user trả lời k
                if(answer==null) continue; // answerId không hợp lệ cho câu này -> coi như sai/bỏ qua
                bool isCorrect=answer.IsCorrect;
                if(isCorrect) correctAnswers++; // nếu đúng thì cộng vào tính là 1 
                if(existingByQuestionId.TryGetValue(question.Id,out var existing)) // nếu đã làfm câu này r thì ghi lại kq mới
                {
                    existing.LearningAnswerId = answerId;
                    existing.IsCorrect = isCorrect;
                    existing.Score = isCorrect ? 1 : 0;
                    existing.AnsweredAt = DateTime.UtcNow;
                }else // nếu chưa làm thì ghi laại tạo mới
                {
                    _context.LearningUserAnswers.Add(new LearningUserAnswer // luu lai cau user lam vao trong learningUserAnswer
                    {
                        UserId=userId,
                        LearningQuestionId=question.Id,
                        LearningAnswerId=answer.Id,
                        IsCorrect=isCorrect,
                        Score= isCorrect ? 1 : 0 ,
                        AnsweredAt=DateTime.UtcNow
                    });
                }
            }
             // 3. Tính completion dựa trên số câu ĐÃ TỪNG trả lời (không chỉ lần submit này)
            int totalCompletedQuestions = existingByQuestionId.Keys
                .Union(request.Answers.Keys.Where(qId => questions.Any(q => q.Id == qId)))
                .Distinct()
                .Count();
            // topic hoan thanh khi , tong so question >= cac question cac cau hoi dc user tra loi 
            bool isTopicCompleted=totalQuestions >0 && totalCompletedQuestions >=totalQuestions;
            int completionPercentage= totalQuestions ==0 ?0 : (int)((double)totalCompletedQuestions/totalQuestions * 100);
            var userProgress= await _context.UserProgresses.FirstOrDefaultAsync(up=>up.UserId==userId && up.TopicId==request.TopicId);

            if (userProgress == null)
            {
                userProgress = new UserProgress
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

            await _context.SaveChangesAsync(); // save lai 
            // tinh diem 
            double score = totalQuestions > 0 ? (double)correctAnswers / totalQuestions * 100 : 0; // tinh diem bang cach tong so dap an dung/tong so cau hoi *100

            var topicProgressDto = await MapToTopicSummaryDto(topic, userId);
             SubjectSummaryDto? subjectProgressDto = isTopicCompleted
                ? await MapToSubjectSummaryDto(topic.Subject, userId)
                : null;
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
