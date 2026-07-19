using Microsoft.EntityFrameworkCore;
using Source.Data;
using Source.DTOs.Admin;
using Source.Middleware;
using Source.Models;
using Source.Service.Admin.Interface;
namespace Source.Service.Admin
{
    public class AdminRecommendationQuestions: IAdminRecoQuestions
    {
        private readonly ApplicationDbContext _context;
        public AdminRecommendationQuestions(ApplicationDbContext context)
        {
            _context = context;
        }

        // RecommendationQuestion CRUD
        public async Task<List<AdminRecommendationQuestionDto>> GetAllRecommendationQuestions()
        {
            return await _context.RecommendationQuestions
                .Include(rq => rq.Quiz)
                .Select(rq => new AdminRecommendationQuestionDto
                {
                    Id = rq.Id,
                    Content = rq.Content,
                    QuizId = rq.QuizId,
                    QuizTitle = rq.Quiz.Title
                })
                .ToListAsync();
        }

        public async Task<AdminRecommendationQuestionDto?> GetRecommendationQuestionById(int id)
        {
            var question = await _context.RecommendationQuestions
                .Include(rq => rq.Quiz)
                .FirstOrDefaultAsync(rq => rq.Id == id);

            if (question == null) return null;

            return new AdminRecommendationQuestionDto
            {
                Id = question.Id,
                Content = question.Content,
                QuizId = question.QuizId,
                QuizTitle = question.Quiz.Title
            };
        }



        public async Task<AdminRecoDetailQuestion_listAnswerDto> GetDetailQuestion_ListAnswers(int id) // vì là details nên cần id 
        {
            var data = await _context.RecommendationQuestions.Include(p => p.Answers).FirstOrDefaultAsync(q => q.Answers.Any());
            if (data== null) throw new BadRequestException("Valid data cannot get bruhhh ");
            // get list answer 
            var answers = await _context.RecommendationAnswers.Where(p => p.RecommendationQuestionId == data.Id).ToListAsync();
            // tra ve dto 
            var dto = new AdminRecoDetailQuestion_listAnswerDto
            {
                Id = data.Id,
                QuizId = data.QuizId,
                content = data.Content,
                Answers = answers.Select(a => new AdminRecommendationAnswerDto
                {
                    Id = a.Id,
                    Content = a.Content,
                    RecommendationQuestionId = data.Id,
                    QuestionContent = data.Content,
                }).ToList()
            };
            if (dto == null) throw new BadRequestException(" Error Cannot get data bruhh  ");
            return dto;
            
        }

        public async Task<AdminRecommendationQuestionDto> CreateRecommendationQuestion(CreateRecommendationQuestionDto dto)
        {
            // Check if Quiz exists
            var quiz = await _context.Quizzes.FindAsync(dto.QuizId);
            if (quiz == null)
            {
                throw new NotFoundException($"Quiz with ID {dto.QuizId} not found");
            }

            var question = new Models.RecommendationQuestion
            {
                Content = dto.Content,
                QuizId = dto.QuizId
            };

            _context.RecommendationQuestions.Add(question);
            await _context.SaveChangesAsync();

            return new AdminRecommendationQuestionDto
            {
                Id = question.Id,
                Content = question.Content,
                QuizId = question.QuizId,
                QuizTitle = quiz.Title
            };
        }

        public async Task<AdminRecommendationQuestionDto?> UpdateRecommendationQuestion(int id, UpdateRecommendationQuestionDto dto)
        {
            var question = await _context.RecommendationQuestions.FindAsync(id);
            if (question == null) return null;

            // Check if Quiz exists
            var quiz = await _context.Quizzes.FindAsync(dto.QuizId);
            if (quiz == null)
            {
                throw new NotFoundException($"Quiz with ID {dto.QuizId} not found");
            }

            question.Content = dto.Content;
            question.QuizId = dto.QuizId;

            await _context.SaveChangesAsync();

            return new AdminRecommendationQuestionDto
            {
                Id = question.Id,
                Content = question.Content,
                QuizId = question.QuizId,
                QuizTitle = quiz.Title
            };
        }

        public async Task<bool> DeleteRecommendationQuestion(int id)
        {
            var question = await _context.RecommendationQuestions.FindAsync(id);
            if (question == null) return false;

            // Check if question has related answers
            if (await _context.RecommendationAnswers.AnyAsync(ra => ra.RecommendationQuestionId == id))
            {
                throw new BadRequestException("Cannot delete question that has related answers");
            }

            // Check if question has related user answers
            if (await _context.RecommendationUserAnswers.AnyAsync(rua => rua.RecommendationQuestionId == id))
            {
                throw new BadRequestException("Cannot delete question that has related user answers");
            }

            _context.RecommendationQuestions.Remove(question);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
