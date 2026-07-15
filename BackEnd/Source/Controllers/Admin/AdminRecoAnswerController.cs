using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin;
using Source.Service.Admin.Interface;

namespace Source.Controllers.Admin
{
<<<<<<< HEAD
    public class AdminRecoAnswerController : BaseAdminController
=======
    public class AdminRecoAnswerController : BaseController
>>>>>>> 4c338964ceab40710cfd71caa92fa05a73ce5c73
    {
        private readonly IAdminRecoAnswers _recoAnswer;
        public AdminRecoAnswerController(IAdminRecoAnswers recoAnswer)
        {
            _recoAnswer = recoAnswer;
        }
        // RecommendationAnswer CRUD
        [HttpGet("answers")]
        public async Task<IActionResult> GetAllRecommendationAnswers()
        {
            var answers = await _recoAnswer.GetAllRecommendationAnswers();
            return Success(answers,"Get all Recommendation Answer Success");
        }

        [HttpGet("answers/{id}")]
        public async Task<IActionResult> GetRecommendationAnswerById(int id)
        {
            var answer = await _recoAnswer.GetRecommendationAnswerById(id);
            if (answer == null)
            {
                return NotFound(new { message = "Answer not found" });
            }
            return Success(answer," Get Specific answer Success");
        }

        [HttpPost("answers")]
        public async Task<IActionResult> CreateRecommendationAnswer([FromBody] CreateRecommendationAnswerDto dto)
        {
            var answer = await _recoAnswer.CreateRecommendationAnswer(dto);
            return CreatedAtAction(nameof(GetRecommendationAnswerById), new { id = answer.Id }, Success(answer));
        }

        [HttpPut("answers/{id}")]
        public async Task<IActionResult> UpdateRecommendationAnswer(int id, [FromBody] UpdateRecommendationAnswerDto dto)
        {
            var answer = await _recoAnswer.UpdateRecommendationAnswer(id, dto);
            if (answer == null)
            {
                return NotFound(new { message = "Answer not found" });
            }
            return Success(answer," Edit Answer reco Success");
        }

        [HttpDelete("answers/{id}")]
        public async Task<IActionResult> DeleteRecommendationAnswer(int id)
        {
            var result = await _recoAnswer.DeleteRecommendationAnswer(id);
            if (!result)
            {
                return NotFound(new { message = "Answer not found" });
            }
            return Success(" Xoa thanh cong ");
        }
    }
}
