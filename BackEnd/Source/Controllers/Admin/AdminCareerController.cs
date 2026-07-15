using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;

namespace Source.Controllers.Admin
{
<<<<<<< HEAD
  
    public class AdminCareerController : BaseAdminController
=======
    [ApiController]
    [Route("api/admin/[controller]")]
    public class AdminCareerController : BaseController
>>>>>>> 4c338964ceab40710cfd71caa92fa05a73ce5c73
    {
        private readonly IAdminCareerService _adminCareerService;

        public AdminCareerController(IAdminCareerService adminCareerService)
        {
            _adminCareerService = adminCareerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCareers()
        {
            var careers = await _adminCareerService.GetAllCareers();
            return Success(careers," Get all success");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCareerById(int id)
        {
            var career = await _adminCareerService.GetCareerById(id);
            if (career == null)
            {
                return NotFound(new { message = "Career not found" });
            }
            return Success(career," Get successs");
        }

        [HttpPost]
        public async Task<IActionResult> CreateCareer([FromBody] CreateCareerDto dto)
        {
            var career = await _adminCareerService.CreateCareer(dto);
            return CreatedAtAction(nameof(GetCareerById), new { id = career.Id }, Success(career," Create Career Sucess"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCareer(int id, [FromBody] UpdateCareerDto dto)
        {
            var career = await _adminCareerService.UpdateCareer(id, dto);
            if (career == null)
            {
                return NotFound(new { message = "Career not found" });
            }
            return Success(career,"Success");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCareer(int id)
        {
            var result = await _adminCareerService.DeleteCareer(id);
            if (!result)
            {
                return NotFound(new { message = "Career not found" });
            }
            return Success(" Delete Success ");
        }
    }
}