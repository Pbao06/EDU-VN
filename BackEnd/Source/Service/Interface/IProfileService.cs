using Source.DTOs;
namespace Source.Service.Interface
{
    public interface IProfileService
    {
        Task<UserProfileDto> GetProfileUser(string UserId);
        Task<ProfileEditDto> EditProfileUser(string UserId,ProfileEditDto dto);
    }
}