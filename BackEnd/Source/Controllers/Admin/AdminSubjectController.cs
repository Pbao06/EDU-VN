using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;

namespace Source.Controllers.Admin
{
   
    public class AdminSubjectController : BaseAdminController
    {
        private readonly IAdminSubjectService _adminSubjectService;

        public AdminSubjectController(IAdminSubjectService adminSubjectService)
        {
            _adminSubjectService = adminSubjectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _adminSubjectService.GetAllSubjects();
            return Success(subjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var subject = await _adminSubjectService.GetSubjectById(id);
            if (subject == null)
            {
                return NotFound(new { message = "Subject not found" });
            }
            return Success(subject);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto dto)
        {
            var subject = await _adminSubjectService.CreateSubject(dto);
            return CreatedAtAction(nameof(GetSubjectById), new { id = subject.Id }, Success(subject));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateSubjectDto dto)
        {
            var subject = await _adminSubjectService.UpdateSubject(id, dto);
            if (subject == null)
            {
                return NotFound(new { message = "Subject not found" });
            }
            return Success(subject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var result = await _adminSubjectService.DeleteSubject(id);
            if (!result)
            {
                return NotFound(new { message = "Subject not found" });
            }
            return Success(" Xoa dc r ");
        }
    }
}