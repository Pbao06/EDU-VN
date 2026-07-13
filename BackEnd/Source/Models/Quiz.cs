namespace Source.Models
{
    /// <summary>
    /// Quiz cho Career Recommendation
    /// Chỉ phục vụ duy nhất cho việc định hướng nghề nghiệp
    /// </summary>
    public class Quiz
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;         // "QUIZ_CNTT", "QUIZ_MARKETING", "QUIZ_KINH_TE" - Unique
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        //public int SubjectId { get; set; }  // Placeholder - có thể bỏ sau này
        public int? FieldId { get; set; }  // FK → Field (cho Career Recommendation)
        public int DurationMinutes { get; set; }

        // Navigation Properties
       
        public Field? Field { get; set; }  // Navigation đến Field
        public ICollection<RecommendationQuestion> Questions { get; set; } = new List<RecommendationQuestion>();
        public ICollection<RecommendationUserAnswer> UserAnswers { get; set; } = new List<RecommendationUserAnswer>();
        public ICollection<QuizCareerRecommendation> QuizCareerRecommendations { get; set; } = new List<QuizCareerRecommendation>();
    }
}
