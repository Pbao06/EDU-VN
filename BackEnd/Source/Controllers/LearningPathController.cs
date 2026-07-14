using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Source.DTOs;
using Source.Service.Interface;

namespace Source.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]

    public class LearningPathController : BaseController
    {
        private readonly ILearningPathService _learningPathService;
        

        public LearningPathController(ILearningPathService learningPathService)
        {
            _learningPathService = learningPathService;
           
        }

        // ==================== LEARNING PATH LEVEL ====================

        /// <summary>
        /// Tạo learning path mới cho user với career đã chọn
        /// </summary>
        [HttpPost("{careerId}/start")]
        public async Task<IActionResult> StartLearningPath(int careerId, [FromBody] CreateLearningPathDto? request = null)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không tìm thấy User ID trong token");
            }

            var title = request?.Title;
            var result = await _learningPathService.StartLearningPath(userId, careerId, title);
            return Success(result, result.Message);
        }

        /// <summary>
        /// Lấy tất cả learning paths của user (summary only)
        /// </summary>
        [HttpGet("user")]
        public async Task<IActionResult> GetUserLearningPaths()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không tìm thấy User ID trong token");
            }

            var learningPaths = await _learningPathService.GetUserLearningPaths(userId);
            return Success(learningPaths, "Lấy danh sách learning paths thành công");
        }

        /// <summary>
        /// Lấy chi tiết learning path với Subjects list (không có Topics)
        /// </summary>
        [HttpGet("{learningPathId}")]
        public async Task<IActionResult> GetLearningPathDetail(int learningPathId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không tìm thấy User ID trong token");
            }

            var detail = await _learningPathService.GetLearningPathDetail(learningPathId, userId);
            return Success(detail, "Lấy chi tiết learning path thành công");
        }

        
    }
}
