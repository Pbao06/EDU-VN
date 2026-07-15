using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;
using Source.Controllers.Admin;

namespace Source.Controllers.Admin
{
    
    public class AdminCareerSubjectController : BaseAdminController
    {
        private readonly IAdminCareerSubjectService _adminCareerSubjectService;

        public AdminCareerSubjectController(IAdminCareerSubjectService adminCareerSubjectService)
        {
            _adminCareerSubjectService = adminCareerSubjectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCareerSubjects()
        {
            var careerSubjects = await _adminCareerSubjectService.GetAllCareerSubjects();
            return Success(careerSubjects," lay thanh cong list");
        }

        [HttpGet("{careerId}/{subjectId}")]
        public async Task<IActionResult> GetCareerSubjectById(int careerId, int subjectId)
        {
            var careerSubject = await _adminCareerSubjectService.GetCareerSubjectById(careerId, subjectId);
            if (careerSubject == null)
            {
                return NotFound(new { message = "CareerSubject not found" });
            }
            return Success(careerSubject, " Lay duoc ");
        }

        [HttpPost]
        public async Task<IActionResult> CreateCareerSubject([FromBody] CreateCareerSubjectDto dto)
        {
            var careerSubject = await _adminCareerSubjectService.CreateCareerSubject(dto);
            return CreatedAtAction(nameof(GetCareerSubjectById), new { careerId = careerSubject.CareerId, subjectId = careerSubject.SubjectId }, Success(careerSubject," Create thanh cong"));
        }

        [HttpPut("{careerId}/{subjectId}")]
        public async Task<IActionResult> UpdateCareerSubject(int careerId, int subjectId, [FromBody] UpdateCareerSubjectDto dto)
        {
            var careerSubject = await _adminCareerSubjectService.UpdateCareerSubject(careerId, subjectId, dto);
            if (careerSubject == null)
            {
                return NotFound(new { message = "CareerSubject not found" });
            }
            return Success(careerSubject," Lay dc roi ");
        }

        [HttpDelete("{careerId}/{subjectId}")]
        public async Task<IActionResult> DeleteCareerSubject(int careerId, int subjectId)
        {
            var result = await _adminCareerSubjectService.DeleteCareerSubject(careerId, subjectId);
            if (!result)
            {
                return NotFound(new { message = "CareerSubject not found" });
            }
            return Success(" Xoa dc roi ");
        }
    }
}