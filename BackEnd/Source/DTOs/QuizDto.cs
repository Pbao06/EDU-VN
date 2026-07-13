namespace Source.DTOs
{
    /// <summary>
    /// DTO cho Quiz - dùng cho cả basic và detail
    /// </summary>
    public class QuizDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        
        // Optional - chỉ có khi lấy chi tiết quiz
        public List<RecommendationQuestionDto>? Questions { get; set; }
    }

    /// <summary>
    /// DTO cho Recommendation Question
    /// </summary>
    public class RecommendationQuestionDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int QuizId { get; set; }
        public List<RecommendationAnswerDto> Answers { get; set; } = new();
    }

    /// <summary>
    /// DTO cho Recommendation Answer
    /// </summary>
    public class RecommendationAnswerDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int RecommendationQuestionId { get; set; }
    }

    /// <summary>
    /// DTO cho request nộp bài quiz
    /// </summary>
    public class QuizSubmitRequestDto
    {
        /// <summary>
        /// Danh sách các câu trả lời của user
        /// Key: QuestionId, Value: AnswerId
        /// </summary>
        public Dictionary<int, int> Answers { get; set; } = new();
    }

    /// <summary>
    /// DTO cho kết quả quiz đơn giản
    /// </summary>
    public class QuizResultDto
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public List<CareerResultDto> Careers { get; set; } = new();
        public DateTime SubmittedAt { get; set; }
    }

    /// <summary>
    /// DTO cho career result đơn giản
    /// </summary>
    public class CareerResultDto
    {
        public int CareerId { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public double MatchPercentage { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }
}
