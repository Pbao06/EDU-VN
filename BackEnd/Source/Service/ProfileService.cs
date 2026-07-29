using Source.Service.Interface;
using Source.Data;
using Source.DTOs;
using Microsoft.EntityFrameworkCore;
using Source.Middleware;
using Source.Models;
using Source.Models.Enums;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
namespace Source.Service
{
    public class ProfileService: IProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public ProfileService(ApplicationDbContext context,UserManager<User> userManager)
        {
            _context=context; _userManager=userManager;

        }

        // co 3 API can : get info usser , put edit password user, edit profile
        public async Task<UserProfileDto> GetProfileUser(string UserId)
        {
            var user= await _context.Users.Include(p=>p.Field).FirstOrDefaultAsync(u=>u.Id==UserId);
            if(user==null) throw new NotFoundException("Not found User");
            var roles= await _userManager.GetRolesAsync(user);
            if(roles==null) throw new BadRequestException("User role null");
            var data= new UserProfileDto
            {
                UserId=user.Id,
                FullName=user.FullName,
                Email=user.Email,
                AvatarUrl=user.AvatarUrl,
                FieldName=user.Field.Name,
                MainGoal=user.MainGoal.ToString()!,
                UseType=user.UserType.ToString()!,
                UpdatedAt=user.UpdatedAt,
                FieldId=user.FieldId,
                Role=roles.FirstOrDefault()
            }; 
            // ko check cho logger no ghi lai if error
            return data;
        }
        //Edit Profile 
        public async Task<ProfileEditDto> EditProfileUser(string UserId,ProfileEditDto dto)
        {
            var user= await _context.Users.FindAsync(UserId);
            if(user==null) throw new NotFoundException("Not found user");
            user.Email=dto.Email;
            user.AvatarUrl=dto.AvatarUrl;
            user.FieldId=dto.FieldId;
            if (!Enum.TryParse<MainGoal>(dto.Maingoal,true, out var mainGoal))// true la ignore viet hoa viet thuong
            {
                throw new BadRequestException("Main Goal không hợp lệ.");
            }
            user.MainGoal = mainGoal;
            if(!Enum.TryParse<UserType>(dto.UseType,true, out var usetype))
            {
                throw new BadRequestException(" UseType khong hop le");
            }
            user.UserType=usetype;
            user.UpdatedAt=DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var data = new ProfileEditDto
            {
                FullName=user.FullName,
                Email=user.Email,
                FieldId=user.FieldId,
                UseType=user.UserType.ToString(),
                Maingoal=user.MainGoal.ToString(),
                UpdatedAt=user.UpdatedAt,
                AvatarUrl=user.AvatarUrl
            };
            return data;
        }
        // edit password 
    }
}