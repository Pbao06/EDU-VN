namespace Source.DTOs.Admin
{
    public class AdminQuizDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? FieldId { get; set; }
        public string? FieldName { get; set; }
        public int DurationMinutes { get; set; }
    }

    public class CreateQuizDto
    {
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? FieldId { get; set; }
        public int DurationMinutes { get; set; }
    }

    public class UpdateQuizDto
    {
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? FieldId { get; set; }
        public int DurationMinutes { get; set; }
    }

    public class AdminRecommendationQuestionDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
    }

    public class CreateRecommendationQuestionDto
    {
        public string Content { get; set; } = string.Empty;
        public int QuizId { get; set; }
    }

    public class UpdateRecommendationQuestionDto
    {
        public string Content { get; set; } = string.Empty;
        public int QuizId { get; set; }
    }

    public class AdminRecommendationAnswerDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int RecommendationQuestionId { get; set; }
        public string QuestionContent { get; set; } = string.Empty;
    }

    public class CreateRecommendationAnswerDto
    {
        public string Content { get; set; } = string.Empty;
        public int RecommendationQuestionId { get; set; }
    }

    public class UpdateRecommendationAnswerDto
    {
        public string Content { get; set; } = string.Empty;
        public int RecommendationQuestionId { get; set; }
    }
}