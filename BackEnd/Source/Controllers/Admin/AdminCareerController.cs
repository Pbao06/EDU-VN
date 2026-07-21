using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;

namespace Source.Controllers.Admin
{
    public class AdminCareerController : BaseAdminController

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
            var career = await _adminCareerService.GetDetailCareerPublic(id);
            return Success(career, "Get success");
        }

        [HttpPost]
        public async Task<IActionResult> CreateCareer([FromBody] CreateCareerDto dto)
        {
            var career = await _adminCareerService.CreateCareer(dto);
            // We use CreatedAtAction to return 201 Created and the location of the new resource.
            // Note: This won't return the Success envelope, but it's the standard RESTful way.
            return CreatedAtAction(nameof(GetCareerById), new { id = career.Id }, career);
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