namespace Source.Models
{
    /// <summary>
    /// Đáp án cho Learning Question
    /// Có đúng/sai, có explanation
    /// Dùng để kiểm tra kiến thức
    /// </summary>
    public class LearningAnswer
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }  // Có đúng/sai
        public string? Explanation { get; set; }  // Giải thích tại sao đúng/sai
        public int LearningQuestionId { get; set; }  // FK → LearningQuestion
        public int AnswerIndex{get;set;}
        public ICollection<LearningUserAnswer> UserAnswers { get; set; } = new List<LearningUserAnswer>();

        // Navigation Properties
        public LearningQuestion LearningQuestion { get; set; } = null!;
    }
}
