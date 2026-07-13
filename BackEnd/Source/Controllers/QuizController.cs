using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Source.DTOs;
using Source.Service.Interface;

namespace Source.Controllers
{
    /// <summary>
    /// Controller xử lý các request liên quan đến Quiz System
    /// Logic: 1 field = 1 quiz duy nhất
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizController : BaseController
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        /// <summary>
        /// Lấy quiz duy nhất của user với câu hỏi và đáp án
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserQuiz() 
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Không thể xác định người dùng từ token");
            }

            var quiz = await _quizService.GetUserQuizAsync(userId);
            return Success(quiz, "Lấy quiz thành công");
        }

        /// <summary>
        /// Nộp bài quiz và nhận career recommendation
        /// User có thể làm lại nhiều lần
        /// </summary>
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmitRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Không thể xác định người dùng từ token");
            }

            var result = await _quizService.SubmitQuizAsync(userId, request);
            return Success(result, "Nộp bài quiz thành công");
        }

        /// <summary>
        /// Lấy tất cả lịch sử kết quả quiz của user
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetQuizHistory()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Không thể xác định người dùng từ token");
            }

            var history = await _quizService.GetQuizHistoryAsync(userId);
            return Success(history, "Lấy lịch sử quiz thành công");
        }
    }
}
