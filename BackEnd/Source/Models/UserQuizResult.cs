namespace Source.Models
{
    public class UserQuizResult
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;
        public int Score { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
        public string? AnswersJson { get; set; }   // lưu chi tiết câu trả lời
    }
}
