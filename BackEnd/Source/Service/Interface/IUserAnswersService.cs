using Source.Data;
using Source.DTOs;
namespace Source.Service.Interface
{
    public interface IUserAnswersService
    {
        Task<ListUserAnswerDto> GetListUserAnswer(string UserId,int QuizId); // need which field by quizId
    }
}
