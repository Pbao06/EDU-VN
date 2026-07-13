namespace Source.Models
{
    /// <summary>
    /// Câu hỏi kiến thức cho Learning Path
    /// Dùng để kiểm tra kiến thức về Subject/Topic
    /// Có đúng/sai, có explanation, hint, difficulty
    /// </summary>
    public class LearningQuestion
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Explanation { get; set; }  // Giải thích cho câu hỏi
        public string? Hint { get; set; }  // Gợi ý cho user
        public int Difficulty { get; set; }  // 1-5, mức độ khó
        public int TopicId { get; set; }  // FK → Topic
        
        public ICollection<LearningAnswer> Answers { get; set; } = new List<LearningAnswer>();
        public ICollection<LearningUserAnswer> UserAnswers { get; set; } = new List<LearningUserAnswer>();

        // Navigation Properties
        public Topic Topic { get; set; } = null!;
    }
}
