using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;

namespace Source.Controllers.Admin
{
    public class AdminLearningAnswerController : BaseController
    {
        private readonly IAdminLearningAnswers _learningAnswers;
        public AdminLearningAnswerController(IAdminLearningAnswers learningAnswers)
        {
            _learningAnswers = learningAnswers;
        }

        // LearningAnswer CRUD
        [HttpGet("answers")]
        public async Task<IActionResult> GetAllLearningAnswers()
        {
            var answers = await _learningAnswers.GetAllLearningAnswers();
            return Success(answers);
        }

        [HttpGet("answers/{id}")]
        public async Task<IActionResult> GetLearningAnswerById(int id)
        {
            var answer = await _learningAnswers.GetLearningAnswerById(id);
            if (answer == null)
            {
                return NotFound(new { message = "LearningAnswer not found" });
            }
            return Success(answer);
        }

        [HttpPost("answers")]
        public async Task<IActionResult> CreateLearningAnswer([FromBody] CreateLearningAnswerDto dto)
        {
            var answer = await _learningAnswers.CreateLearningAnswer(dto);
            return CreatedAtAction(nameof(GetLearningAnswerById), new { id = answer.Id },Success (answer));
        }

        [HttpPut("answers/{id}")]
        public async Task<IActionResult> UpdateLearningAnswer(int id, [FromBody] UpdateLearningAnswerDto dto)
        {
            var answer = await _learningAnswers.UpdateLearningAnswer(id, dto);
            if (answer == null)
            {
                return NotFound(new { message = "LearningAnswer not found" });
            }
            return Success(answer);
        }

        [HttpDelete("answers/{id}")]
        public async Task<IActionResult> DeleteLearningAnswer(int id)
        {
            var result = await _learningAnswers.DeleteLearningAnswer(id);
            if (!result)
            {
                return NotFound(new { message = "LearningAnswer not found" });
            }
            return Success(" xoa thanh cong ");
        }
    }
}
