namespace Source.Models
{
    public class QuizCareerRecommendation
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        public int CareerId { get; set; }
        public Career Career { get; set; } = null!;

        public double MatchPercentage { get; set; }     // 85.5%
        public string AiExplanation { get; set; } = string.Empty;  // AI giải thích lý do phù hợp
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
