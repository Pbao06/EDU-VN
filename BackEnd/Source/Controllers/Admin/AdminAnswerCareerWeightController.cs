using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;

namespace Source.Controllers.Admin
{
<<<<<<< HEAD
   
    public class AdminAnswerCareerWeightController : BaseAdminController
=======
    [ApiController]
    [Route("api/admin/[controller]")]
    public class AdminAnswerCareerWeightController : BaseController
>>>>>>> 4c338964ceab40710cfd71caa92fa05a73ce5c73
    {
        private readonly IAdminAnswerCareerWeightService _adminAnswerCareerWeightService;

        public AdminAnswerCareerWeightController(IAdminAnswerCareerWeightService adminAnswerCareerWeightService)
        {
            _adminAnswerCareerWeightService = adminAnswerCareerWeightService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAnswerCareerWeights()
        {
            var weights = await _adminAnswerCareerWeightService.GetAllAnswerCareerWeights();
            return Success(weights);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswerCareerWeightById(int id)
        {
            var weight = await _adminAnswerCareerWeightService.GetAnswerCareerWeightById(id);
            if (weight == null)
            {
                return NotFound(new { message = "AnswerCareerWeight not found" });
            }
            return Success(weight);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnswerCareerWeight([FromBody] CreateAnswerCareerWeightDto dto)
        {
            var weight = await _adminAnswerCareerWeightService.CreateAnswerCareerWeight(dto);
            return CreatedAtAction(nameof(GetAnswerCareerWeightById), new { id = weight.Id }, Success(weight));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnswerCareerWeight(int id, [FromBody] UpdateAnswerCareerWeightDto dto)
        {
            var weight = await _adminAnswerCareerWeightService.UpdateAnswerCareerWeight(id, dto);
            if (weight == null)
            {
                return NotFound(new { message = "AnswerCareerWeight not found" });
            }
            return Success(weight);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswerCareerWeight(int id)
        {
            var result = await _adminAnswerCareerWeightService.DeleteAnswerCareerWeight(id);
            if (!result)
            {
                return NotFound(new { message = "AnswerCareerWeight not found" });
            }
            return Success(" Xoa thanh cong ");
        }
    }
}