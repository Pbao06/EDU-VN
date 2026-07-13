using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Source.Service.Interface;

namespace Source.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]

    public class CareerController : BaseController
    {
        private readonly ICareerService _careerService;
        public CareerController(ICareerService careerService)
        {
            _careerService= careerService;
        }
        // get list user
        [HttpGet("GetListCareer")]
        public async Task<IActionResult> GetListCareer()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không tìm thấy User ID trong token");
            }
            var listcareer = await _careerService.GetListCareer(userId);
            return Success(listcareer, "Danh sách career gửi thành công");

        }
        [HttpGet("GetDetailCareer/{id}")]
        public async Task<IActionResult> GetDetailCareer(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không tìm thấy User ID trong token");
            }
            var detail = await _careerService.GetDetailCareer(userId,id);
            return Success(detail, "Lấy detail career thành công");
        }
    }
}
