using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;

namespace Source.Controllers.Admin
{
<<<<<<< HEAD
    
    public class AdminQuizController : BaseAdminController
=======
    [ApiController]
    [Route("api/admin/[controller]")]
    public class AdminQuizController : BaseController
>>>>>>> 4c338964ceab40710cfd71caa92fa05a73ce5c73
    {
        private readonly IAdminQuizService _adminQuizService;

        public AdminQuizController(IAdminQuizService adminQuizService)
        {
            _adminQuizService = adminQuizService;
        }

        // Quiz CRUD
        [HttpGet("quizzes")]
        public async Task<IActionResult> GetAllQuizzes()
        {
            var quizzes = await _adminQuizService.GetAllQuizzes();
            return Success(quizzes," Get list all successs");
        }

        [HttpGet("quizzes/{id}")]
        public async Task<IActionResult> GetQuizById(int id)
        {
            var quiz = await _adminQuizService.GetQuizById(id);
            if (quiz == null)
            {
                return NotFound(new { message = "Quiz not found" });
            }
            return Success(quiz," Get Quiz by Id Success");
        }

        [HttpPost("quizzes")]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizDto dto)
        {
            var quiz = await _adminQuizService.CreateQuiz(dto);
            return CreatedAtAction(nameof(GetQuizById), new { id = quiz.Id }, Success(quiz));
        }

        [HttpPut("quizzes/{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, [FromBody] UpdateQuizDto dto)
        {
            var quiz = await _adminQuizService.UpdateQuiz(id, dto);
            if (quiz == null)
            {
                return NotFound(new { message = "Quiz not found" });
            }
            return Success(quiz," Edit Quiz Sucesss ");
        }

        [HttpDelete("quizzes/{id}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var result = await _adminQuizService.DeleteQuiz(id);
            if (!result)
            {
                return NotFound(new { message = "Quiz not found" });
            }
            return Success(" Delete Success");
        }

   
    }
}