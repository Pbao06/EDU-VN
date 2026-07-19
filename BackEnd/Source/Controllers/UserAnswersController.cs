using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Source.Service.Interface;
using System.Runtime.InteropServices.Marshalling;
namespace Source.Controllers
{
    [ApiController]
    [Authorize] // need token to get here
    // dont need rout/controller cause father class got it
    public class UserAnswersController : BaseController
    {
        private readonly IUserAnswersService _userAnswersService;
        //init it 
        public UserAnswersController(IUserAnswersService userAnswersService) => _userAnswersService = userAnswersService;
        // get list user Answer 
        [HttpGet("GetUserAnswer/{QuizId}")]
        public async Task<IActionResult> GetListUserAnswer(int QuizId)
        {
            // id user 
            var userId= User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _userAnswersService.GetListUserAnswer(userId,QuizId);
            return Success(result, " Get user answers success ");
        }
    }
}
