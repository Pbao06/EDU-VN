namespace Source.Models
{
    /// <summary>
    /// Câu hỏi trắc nghiệm định hướng nghề nghiệp
    /// Chỉ dùng cho Career Recommendation Quiz
    /// Không có đúng/sai, chỉ để thu thập preference của user
    /// </summary>
    public class RecommendationQuestion
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int QuizId { get; set; }  // FK → Quiz (Recommendation Quiz)
        
        public ICollection<RecommendationAnswer> Answers { get; set; } = new List<RecommendationAnswer>();
        public ICollection<RecommendationUserAnswer> UserAnswers { get; set; } = new List<RecommendationUserAnswer>();

        // Navigation Properties
        public Quiz Quiz { get; set; } = null!;
    }
}
