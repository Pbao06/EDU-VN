using System.ComponentModel.DataAnnotations;
using Source.Models.Enums;
namespace Source.DTOs
{
    public class UserProfileDto
    {
        [Required]
        public string UserId{get;set;}
        [Required(ErrorMessage ="Email khong duoc de trong")] 
        public string Email{get;set;}
        [Required(ErrorMessage=" User Name khong null")]
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? UseType{get;set;}
        public string? MainGoal{get;set;}
        public string? Role{get;set;}
        public int? FieldId{get;set;}
        public string? FieldName{get;set;}
        public DateTime? UpdatedAt { get; set; }

    }
    public class ProfileEditDto
    {
        public string FullName { get; set; } = string.Empty;
        public int? FieldId{get;set;}
        public string? UseType{get;set;}
        public string? Maingoal{get;set;}
        public DateTime? UpdatedAt { get; set; }
    }
}