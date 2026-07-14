namespace Source.DTOs.Admin
{
    public class AdminLearningQuestionDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public string? Hint { get; set; }
        public int Difficulty { get; set; }
        public int TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
    }

    public class CreateLearningQuestionDto
    {
        public string Content { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public string? Hint { get; set; }
        public int Difficulty { get; set; }
        public int TopicId { get; set; }
    }

    public class UpdateLearningQuestionDto
    {
        public string Content { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public string? Hint { get; set; }
        public int Difficulty { get; set; }
        public int TopicId { get; set; }
    }

    public class AdminLearningAnswerDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? Explanation { get; set; }
        public int LearningQuestionId { get; set; }
        public string QuestionContent { get; set; } = string.Empty;
    }

    public class CreateLearningAnswerDto
    {
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? Explanation { get; set; }
        public int LearningQuestionId { get; set; }
    }

    public class UpdateLearningAnswerDto
    {
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? Explanation { get; set; }
        public int LearningQuestionId { get; set; }
    }
}