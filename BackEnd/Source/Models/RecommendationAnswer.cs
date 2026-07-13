namespace Source.Models
{
    /// <summary>
    /// Đáp án cho Recommendation Question
    /// Không có đúng/sai, chỉ dùng để thu thập preference
    /// Mỗi Answer có weight cho các Career thông qua AnswerCareerWeight
    /// </summary>
    public class RecommendationAnswer
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int RecommendationQuestionId { get; set; }  // FK → RecommendationQuestion
        
        public ICollection<AnswerCareerWeight> AnswerCareerWeights { get; set; } = new List<AnswerCareerWeight>();
        public ICollection<RecommendationUserAnswer> UserAnswers { get; set; } = new List<RecommendationUserAnswer>();

        // Navigation Properties
        public RecommendationQuestion RecommendationQuestion { get; set; } = null!;
    }
}
