using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;
namespace Source.Controllers.Admin
{
    public class AdminFieldController : BaseAdminController
    {
        private readonly IAdminFieldService _adminFieldService;

        public AdminFieldController(IAdminFieldService adminFieldService)
        {
            _adminFieldService = adminFieldService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFields()
        {
            var fields = await _adminFieldService.GetAllFields();
            return Success(fields," lay dc r ");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFieldById(int id)
        {
            var field = await _adminFieldService.GetFieldById(id);
            if (field == null)
            {
                return NotFound(new { message = "Field not found" });
            }
            return Success(field," lay dc r ");
        }

        [HttpPost]
        public async Task<IActionResult> CreateField([FromBody] CreateFieldDto dto)
        {
            var field = await _adminFieldService.CreateField(dto);
            return CreatedAtAction(nameof(GetFieldById), new { id = field.Id }, Success(field," Create dc r "));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateField(int id, [FromBody] UpdateFieldDto dto)
        {
            var field = await _adminFieldService.UpdateField(id, dto);
            if (field == null)
            {
                return NotFound(new { message = "Field not found" });
            }
            return Success(field," edit dc r ");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteField(int id)
        {
            var result = await _adminFieldService.DeleteField(id);
            if (!result)
            {
                return NotFound(new { message = "Field not found" });
            }
            return Success(" Xoa dc r ");
        }
    }
}