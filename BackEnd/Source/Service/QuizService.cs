using Microsoft.EntityFrameworkCore;
using Source.Data;
using Source.DTOs;
using Source.Middleware;
using Source.Models;
using Source.Service.Interface;

namespace Source.Service
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _context;

        public QuizService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<QuizDto> GetUserQuizAsync(string userId) // lay toan bo cau hoi + answer + quizz cua User
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.FieldId == null)
            {
                throw new NotFoundException("User không tồn tại hoặc chưa chọn field");
            }

            var quiz = await _context.Quizzes // (field)quiz - question - answer
                .Include(q => q.Field)
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.FieldId == user.FieldId);

            if (quiz == null)
            {
                throw new NotFoundException("Không có quiz cho field này");
            }

            return new QuizDto
            {
                Id = quiz.Id,
                Code = quiz.Code,
                Title = quiz.Title,
                Description = quiz.Description ?? string.Empty,
                FieldId = quiz.FieldId ?? 0,
                FieldName = quiz.Field != null ? quiz.Field.Name : string.Empty,
                DurationMinutes = quiz.DurationMinutes,
                Questions = quiz.Questions.Select(q => new RecommendationQuestionDto
                {
                    Id = q.Id,
                    Content = q.Content,
                    QuizId = q.QuizId,
                    Answers = q.Answers.Select(a => new RecommendationAnswerDto
                    {
                        Id = a.Id,
                        Content = a.Content,
                        RecommendationQuestionId = a.RecommendationQuestionId
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<QuizResultDto> SubmitQuizAsync(string userId, QuizSubmitRequestDto request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.FieldId == null)
            {
                throw new NotFoundException("User không tồn tại hoặc chưa chọn field");
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Field)
                .FirstOrDefaultAsync(q => q.FieldId == user.FieldId);

            if (quiz == null)
            {
                throw new NotFoundException("Không có quiz cho field này");
            }

            if (request.Answers == null || request.Answers.Count == 0)
            {
                throw new BadRequestException("Không có câu trả lời nào được nộp");
            }

            var quizQuestions = await _context.RecommendationQuestions
                .Where(q => q.QuizId == quiz.Id)
                .ToListAsync();

            foreach (var question in quizQuestions) // tìm xem trong đống câu hỏi đã có xêm User đã trả lời đủ chưa 
            {
                if (!request.Answers.ContainsKey(question.Id))
                {
                    throw new BadRequestException($"Câu hỏi {question.Id} chưa được trả lời");
                }
            }

            var answeredAt = DateTime.UtcNow;
            List<CareerResultDto> careerResults = null!;

           // use transaction 
           var strategy = _context.Database.CreateExecutionStrategy();
           await strategy.ExecuteAsync(async () =>
           {
               using var transaction=await _context.Database.BeginTransactionAsync();
                try
                {

                    // Xóa dữ liệu cũ nếu user nộp lại bài (để tránh trùng lặp khi retry chạy lại)
                        var existingAnswers = await _context.RecommendationUserAnswers
                            .Where(x => x.UserId == userId && x.QuizId == quiz.Id)
                            .ToListAsync();
                        if (existingAnswers.Any())
                        {
                            _context.RecommendationUserAnswers.RemoveRange(existingAnswers);
                        }

                        var existingRecs = await _context.QuizCareerRecommendations
                            .Where(x => x.UserId == userId && x.QuizId == quiz.Id)
                            .ToListAsync();
                        if (existingRecs.Any())
                        {
                            _context.QuizCareerRecommendations.RemoveRange(existingRecs);
                        }
                    foreach (var answer in request.Answers)
                        {
                            _context.RecommendationUserAnswers.Add(new RecommendationUserAnswer
                            {
                                UserId = userId,
                                QuizId = quiz.Id,
                                RecommendationQuestionId = answer.Key,
                                RecommendationAnswerId = answer.Value,
                                AnsweredAt = answeredAt
                            });
                        }
                        await _context.SaveChangesAsync();

                        // Calculate career recommendations (may read DB)
                        careerResults = await Caculate(quiz.Id, user.Id);

                    // Insert career recommendations
                        foreach (var career in careerResults)
                        {
                            _context.QuizCareerRecommendations.Add(new QuizCareerRecommendation
                                {
                                    UserId = userId,
                                    QuizId = quiz.Id,
                                    CareerId = career.CareerId,
                                    MatchPercentage = career.MatchPercentage,
                                    AiExplanation = career.Explanation,
                                    CreatedAt = answeredAt
                                });
                        }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                } 
           });
           return new QuizResultDto
                    {
                        QuizId = quiz.Id,
                        QuizTitle = quiz.Title,
                        Careers = careerResults,
                        SubmittedAt = answeredAt
                    };
           
        }
        public async Task<List<CareerResultDto>> Caculate(int quizId, string UserId)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
            {
                throw new NotFoundException("Not found user");
            }

            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == quizId && q.FieldId == user.FieldId);
            if (quiz == null)
            {
                throw new BadRequestException("Quiz không thuộc field của user");
            }

            var scoreByCareer = await _context.RecommendationUserAnswers
                .Where(a => a.UserId == user.Id && a.QuizId == quiz.Id)
                .SelectMany(ua => ua.RecommendationAnswer.AnswerCareerWeights)
                .GroupBy(acw => acw.CareerId)
                .Select(g => new
                {
                    CareerId = g.Key,
                    TotalScore = g.Sum(x => x.Weight)
                })
                .ToListAsync();

            var careers = await _context.Careers
                .Where(c => c.FieldId == quiz.FieldId)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (careers.Count == 0)
            {
                return new List<CareerResultDto>();
            }

            var PositiveScore = scoreByCareer
                .Where(x => x.TotalScore > 0).ToList();
            var totalPositiveScore= PositiveScore.Sum(x=>x.TotalScore);
            var results = careers.Select(career =>
            {
                var score = scoreByCareer.FirstOrDefault(x => x.CareerId == career.Id)?.TotalScore ?? 0;
                double rawPercentage = totalPositiveScore <= 0 || score <= 0 ? 0
                : (double)score / totalPositiveScore * 100;
                double matchPercentage = Math.Round(Math.Min(99.99, Math.Max(0, rawPercentage)), 2);

                return new CareerResultDto
                {
                    CareerId = career.Id,
                    CareerName = career.Name,
                    MatchPercentage = matchPercentage,
                    Explanation = GenerateExplanation(career.Name, matchPercentage)
                };
            }).ToList();

            return results
                .OrderByDescending(x => x.MatchPercentage)
                .ThenBy(x => x.CareerId)
                .ToList();
        }

        public async Task<List<QuizResultDto>> GetQuizHistoryAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.FieldId == null)
            {
                throw new NotFoundException("User không tồn tại hoặc chưa chọn field");
            }

            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(q => q.FieldId == user.FieldId);

            if (quiz == null)
            {
                throw new NotFoundException("Không có quiz cho field này");
            }

            var allResults = await _context.QuizCareerRecommendations
                .Where(qcr => qcr.UserId == userId && qcr.QuizId == quiz.Id)
                .Include(qcr => qcr.Quiz)
                .Include(qcr => qcr.Career)
                    .ThenInclude(c => c.Field)
                .OrderByDescending(qcr => qcr.CreatedAt)
                .ToListAsync();

            var history = new List<QuizResultDto>();

            foreach (var resultGroup in allResults.GroupBy(qcr => qcr.CreatedAt))
            {
                var careerResults = resultGroup
                    .Select(qcr => new CareerResultDto
                    {
                        CareerId = qcr.CareerId,
                        CareerName = qcr.Career != null ? qcr.Career.Name : string.Empty,
                        // FieldName = qcr.Career != null && qcr.Career.Field != null ? qcr.Career.Field.Name : string.Empty,
                        // Description = qcr.Career != null ? qcr.Career.Description : string.Empty,
                        // MinSalary = qcr.Career != null ? qcr.Career.MinSalary : 0,
                        // MaxSalary = qcr.Career != null ? qcr.Career.MaxSalary : 0,
                        MatchPercentage = qcr.MatchPercentage,
                        Explanation = qcr.AiExplanation ?? string.Empty
                    })
                    .OrderByDescending(c => c.MatchPercentage)
                    .ToList();

                history.Add(new QuizResultDto
                {
                    QuizId = quiz.Id,
                    QuizTitle = quiz.Title,
                    Careers = careerResults,
                    SubmittedAt = resultGroup.Key
                });
            }

            return history;
        }

        // private async Task<List<CareerResultDto>> CalculateCareerRecommendationsAsync(int quizId,Dictionary<int, int> userAnswers)
        // {
        //     // var user= await _context.Users.FindAsync(Userid);
        //     // if(user==null) throw new NotFoundException("Not found user");
        //     var quiz = await _context.Quizzes
        //         .Include(q => q.Field)
        //         .FirstOrDefaultAsync(q => q.Id == quizId); // get dung theo field 

        //     if (quiz?.FieldId == null)
        //     {
        //         throw new NotFoundException("Quiz không tồn tại hoặc không có FieldId");
        //     }


        //     var careers = await _context.Careers
        //         .Where(c => c.FieldId == quiz.FieldId)
        //         .Include(c => c.Field)
        //         .ToListAsync();

        //     // - lấy userAnswer ra join với 
        //     var careerScores = new Dictionary<int, double>();
            
        //     foreach (var career in careers)
        //     {
        //         double totalScore = 0;
                
        //         foreach (var answer in userAnswers)
        //         {
        //             var weight = await _context.AnswerCareerWeights
        //                 .FirstOrDefaultAsync(acw => 
        //                     acw.RecommendationAnswerId == answer.Value && 
        //                     acw.CareerId == career.Id);
                    
        //             if (weight != null)
        //             {
        //                 totalScore += weight.Weight;
        //             }
        //         }
                
        //         careerScores[career.Id] = totalScore; // tính tổng điểm cho mỗi nghề , 
        //     }

        //     var maxScore = careerScores.Values.Max();
        //     var minScore = careerScores.Values.Min();
        //     var range = maxScore - minScore;

        //     var results = new List<CareerResultDto>();

        //     foreach (var career in careers)
        //     {
        //         double matchPercentage = range == 0 ? 50.0 : 
        //             ((careerScores[career.Id] - minScore) / range) * 100;

        //         results.Add(new CareerResultDto
        //         {
        //             CareerId = career.Id,
        //             CareerName = career.Name,
        //             // FieldName = career.Field?.Name ?? string.Empty,
        //             // Description = career.Description,
        //             // MinSalary = career.MinSalary,
        //             // MaxSalary = career.MaxSalary,
        //             MatchPercentage = Math.Round(matchPercentage, 2),
        //             Explanation = GenerateExplanation(Career.Name, matchPercentage)
        //         });
        //     }

        //     return results.OrderByDescending(r => r.MatchPercentage).ToList();
        // }

        private string GenerateExplanation(string CareerName, double matchPercentage)
        {
            var level = matchPercentage > 70 ? "mạnh" : matchPercentage > 50 ? "trung bình" : "khá thấp";
            var explanation = $"Dựa trên câu trả lời của bạn, bạn có tiềm năng {level} cho nghề {CareerName}. ";
            
            // explanation += $"Nghề này trong lĩnh vực {career.Field?.Name ?? career.FieldId.ToString()} có mức lương từ {career.MinSalary:N0} - {career.MaxSalary:N0} VND.";
            
            return explanation;
        }
    }
}
