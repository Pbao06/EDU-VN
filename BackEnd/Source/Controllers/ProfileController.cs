using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Source.DTOs;
using Source.Service;
using Source.Service.Interface;

namespace Source.Controllers
{
    [ApiController]
    [Authorize] // xac thuc token 
    [Route("api/[controller]")]
    public class ProfileController:BaseController
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)=> _profileService=profileService;
        [HttpGet("GetInfo")]
        public async Task<IActionResult> GetProfile()
        {
             var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
             var result= await _profileService.GetProfileUser(userId);
             return Success(result,"Get info success");
        }
        [HttpPut("Edit")]
        public async Task<IActionResult> EditProfile([FromBody] ProfileEditDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result= await _profileService.EditProfileUser(userId,dto);
            return Success(result," Edit thanh cong");
        }
    }
}
