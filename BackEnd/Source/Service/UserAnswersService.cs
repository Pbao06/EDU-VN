using Source.Data;
using Source.DTOs;
using Source.Service.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;
using Source.Models;
namespace Source.Service
{
    public class UserAnswersService:IUserAnswersService
    {
        private readonly ApplicationDbContext _context; 
        //constructor init 
        public UserAnswersService(ApplicationDbContext context)=> _context= context;
        // task 1 : get List userAnser include ( question + answer content) .. 
        public async Task<ListUserAnswerDto> GetListUserAnswer( string UserId, int QuizId)// and user for specific field 
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null) throw new NotFoundException("Not Found User"); 
            var Quiz= await _context.Quizzes.FindAsync(QuizId);
            if (Quiz == null) throw new NotFoundException(" Not Found Quiz for Query");
            var data = await _context.RecommendationUserAnswers.Where(r => r.UserId == user.Id && r.RecommendationQuestion.QuizId == Quiz.Id).Select(r=>new UserAnswerDto
            {
                QuestionId = r.RecommendationQuestionId,
                ContentQuestion = r.RecommendationQuestion.Content,
                AnswerId = r.RecommendationAnswerId,
                ContentAnswer = r.RecommendationAnswer.Content
            }).ToListAsync();

            var dto = new ListUserAnswerDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                UserAnswers = data,
            };
            if (dto==null) throw new BadRequestException(" Something Wrong Cannot select quert this ");
            return dto;
        }
    }
}
