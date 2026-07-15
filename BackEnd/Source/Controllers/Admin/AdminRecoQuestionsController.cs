using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;

namespace Source.Controllers.Admin
{
<<<<<<< HEAD
    public class AdminRecoQuestionsController : BaseAdminController
=======
    public class AdminRecoQuestionsController : BaseController
>>>>>>> 4c338964ceab40710cfd71caa92fa05a73ce5c73
    {
        private readonly IAdminRecoQuestions _recoQuestions;
        public AdminRecoQuestionsController(IAdminRecoQuestions recoQuestions)
        {
            this._recoQuestions = recoQuestions;
        }
        // RecommendationQuestion CRUD
        [HttpGet("questions")]
        public async Task<IActionResult> GetAllRecommendationQuestions()
        {
            var questions = await _recoQuestions.GetAllRecommendationQuestions();
            return Success(questions);
        }

        [HttpGet("questions/{id}")]
        public async Task<IActionResult> GetRecommendationQuestionById(int id)
        {
            var question = await _recoQuestions.GetRecommendationQuestionById(id);
            if (question == null)
            {
                return NotFound(new { message = "Question not found" });
            }
            return Success(question);
        }

        [HttpPost("questions")]
        public async Task<IActionResult> CreateRecommendationQuestion([FromBody] CreateRecommendationQuestionDto dto)
        {
            var question = await _recoQuestions.CreateRecommendationQuestion(dto);
            return CreatedAtAction(nameof(GetRecommendationQuestionById), new { id = question.Id },Success(question));
        }

        [HttpPut("questions/{id}")]
        public async Task<IActionResult> UpdateRecommendationQuestion(int id, [FromBody] UpdateRecommendationQuestionDto dto)
        {
            var question = await _recoQuestions.UpdateRecommendationQuestion(id, dto);
            if (question == null)
            {
                return NotFound(new { message = "Question not found" });
            }
            return Success(question);
        }

        [HttpDelete("questions/{id}")]
        public async Task<IActionResult> DeleteRecommendationQuestion(int id)
        {
            var result = await _recoQuestions.DeleteRecommendationQuestion(id);
            if (!result)
            {
                return NotFound(new { message = "Question not found" });
            }
            return Success(" Xoa thanh cong");
        }

    }
}
