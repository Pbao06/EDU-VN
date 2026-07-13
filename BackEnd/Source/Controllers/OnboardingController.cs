using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Source.DTOs;
using Source.Service.Interface;

namespace Source.Controllers
{
    /// <summary>
    /// Controller xử lý các request liên quan đến onboarding
    /// Sử dụng [Authorize] để yêu cầu người dùng phải đăng nhập
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu authentication (JWT token)
    public class OnboardingController : BaseController
    {
        // Inject OnboardingService thông qua constructor
        private readonly IOnboardingService _onboardingService;

        public OnboardingController(IOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        /// <summary>
        /// Endpoint hoàn thành onboarding
        /// Method: POST
        /// Route: /api/onboarding
        /// </summary>
        /// <param name="dto">Dữ liệu onboarding từ client</param>
        /// <returns>Success response nếu hoàn thành thành công</returns>
        [HttpPost]
        public async Task<IActionResult> CompleteOnboarding([FromBody] OnBoardingDto dto)
        {
            // Bước 1: Validate dữ liệu đầu vào
            // ModelState.IsValid kiểm tra các validation attributes trong DTO
            if (!ModelState.IsValid)
            {
                // Nếu dữ liệu không hợp lệ, trả về BadRequest với chi tiết lỗi
                return BadRequest(ModelState);
            }

            // Bước 2: Lấy userId từ JWT token
            // User.FindFirst(ClaimTypes.NameIdentifier) lấy user ID từ token
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Bước 3: Kiểm tra userId có tồn tại không
            if (string.IsNullOrEmpty(userId))
            {
                // Nếu không lấy được userId từ token, trả về lỗi
                return BadRequest("Không thể xác định người dùng từ token");
            }

            // Bước 4: Gọi service để xử lý logic nghiệp vụ
            // Service sẽ update user trong database
            await _onboardingService.CompleteOnboardingAsync(userId, dto);

            // Bước 5: Trả về response thành công
            // Success() là method từ BaseController, trả về { message, data }
            return Success(null, "Hoàn thành onboarding thành công");
        }

        /// <summary>
        /// Endpoint kiểm tra trạng thái onboarding
        /// Method: GET
        /// Route: /api/onboarding/status
        /// </summary>
        /// <returns>Trạng thái onboarding và dữ liệu nếu đã hoàn thành</returns>
        [HttpGet("status")]
        public async Task<IActionResult> GetOnboardingStatus()
        {
            // Bước 1: Lấy userId từ JWT token
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Bước 2: Kiểm tra userId có tồn tại không
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Không thể xác định người dùng từ token");
            }

            // Bước 3: Gọi service để kiểm tra trạng thái onboarding
            var status = await _onboardingService.IsOnboardingCompletedAsync(userId);

            // Bước 4: Trả về response thành công với dữ liệu trạng thái
            return Success(status, "Lấy trạng thái onboarding thành công");
        }
    }
}