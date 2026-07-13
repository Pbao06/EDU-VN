namespace Source.Models
{
    /// <summary>
    /// Lưu đáp án user chọn khi làm Learning Quiz
    /// Dùng để tracking progress và tính điểm
    /// </summary>
    public class LearningUserAnswer
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;  // FK → User
        public int LearningQuestionId { get; set; }  // FK → LearningQuestion
        public int LearningAnswerId { get; set; }  // FK → LearningAnswer
        public bool IsCorrect { get; set; }  // Computed từ LearningAnswer.IsCorrect
        public int Score { get; set; }  // Điểm cho câu trả lời đúng
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public User User { get; set; } = null!;
        public LearningQuestion LearningQuestion { get; set; } = null!;
        public LearningAnswer LearningAnswer { get; set; } = null!;
    }
}
