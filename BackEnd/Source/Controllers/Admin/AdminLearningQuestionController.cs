using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;

namespace Source.Controllers.Admin
{
<<<<<<< HEAD
   
    public class AdminLearningQuestionController : BaseAdminController
=======
    [ApiController]
    [Route("api/admin/[controller]")]
    public class AdminLearningQuestionController : BaseController
>>>>>>> 4c338964ceab40710cfd71caa92fa05a73ce5c73
    {
        private readonly IAdminLearningQuestionService _adminLearningQuestionService;

        public AdminLearningQuestionController(IAdminLearningQuestionService adminLearningQuestionService)
        {
            _adminLearningQuestionService = adminLearningQuestionService;
        }

        // LearningQuestion CRUD
        [HttpGet("questions")]
        public async Task<IActionResult> GetAllLearningQuestions()
        {
            var questions = await _adminLearningQuestionService.GetAllLearningQuestions();
            return Success(questions);
        }

        [HttpGet("questions/{id}")]
        public async Task<IActionResult> GetLearningQuestionById(int id)
        {
            var question = await _adminLearningQuestionService.GetLearningQuestionById(id);
            if (question == null)
            {
                return NotFound(new { message = "LearningQuestion not found" });
            }
            return Success(question);
        }

        [HttpPost("questions")]
        public async Task<IActionResult> CreateLearningQuestion([FromBody] CreateLearningQuestionDto dto)
        {
            var question = await _adminLearningQuestionService.CreateLearningQuestion(dto);
            return CreatedAtAction(nameof(GetLearningQuestionById), new { id = question.Id }, Success(question));
        }

        [HttpPut("questions/{id}")]
        public async Task<IActionResult> UpdateLearningQuestion(int id, [FromBody] UpdateLearningQuestionDto dto)
        {
            var question = await _adminLearningQuestionService.UpdateLearningQuestion(id, dto);
            if (question == null)
            {
                return NotFound(new { message = "LearningQuestion not found" });
            }
            return Success(question);
        }

        [HttpDelete("questions/{id}")]
        public async Task<IActionResult> DeleteLearningQuestion(int id)
        {
            var result = await _adminLearningQuestionService.DeleteLearningQuestion(id);
            if (!result)
            {
                return NotFound(new { message = "LearningQuestion not found" });
            }
            return Success(" Xoa dc r ");
        }
    }
}