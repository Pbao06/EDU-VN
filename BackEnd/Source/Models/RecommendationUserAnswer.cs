namespace Source.Models
{
    /// <summary>
    /// Lưu đáp án user chọn khi làm Recommendation Quiz
    /// Dùng để tính career recommendation score
    /// </summary>
    public class RecommendationUserAnswer
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;  // FK → User
        public int QuizId { get; set; }  // FK → Quiz
        public int RecommendationQuestionId { get; set; }  // FK → RecommendationQuestion
        public int RecommendationAnswerId { get; set; }  // FK → RecommendationAnswer
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public User User { get; set; } = null!;
        public Quiz Quiz { get; set; } = null!;
        public RecommendationQuestion RecommendationQuestion { get; set; } = null!;
        public RecommendationAnswer RecommendationAnswer { get; set; } = null!;
    }
}
