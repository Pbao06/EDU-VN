using Microsoft.AspNetCore.Identity;
using Source.Models.Enums;

namespace Source.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Thông tin Onboarding - Cập nhật theo schema mới
        public UserType? UserType { get; set; }              // HighSchoolStudent, University, Working
        public MainGoal? MainGoal { get; set; }              // UniversityExam, ImproveGrades, NewSkill, InterviewPrep
        public int? FieldId { get; set; }                     // FK -> Field (CNTT, Marketing, Kinh tế...)
        public bool IsOnboardingCompleted { get; set; } = false;

        // Navigation Properties
        public Field? Field { get; set; }  // Navigation đến Field
        public ICollection<QuizCareerRecommendation> Recommendations { get; set; } = new List<QuizCareerRecommendation>();
        public ICollection<LearningPath> LearningPaths { get; set; } = new List<LearningPath>();
        public ICollection<UserProgress> Progresses { get; set; } = new List<UserProgress>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<RecommendationUserAnswer> RecommendationUserAnswers { get; set; } = new List<RecommendationUserAnswer>();
        public ICollection<LearningUserAnswer> LearningUserAnswers { get; set; } = new List<LearningUserAnswer>();
    }
}
