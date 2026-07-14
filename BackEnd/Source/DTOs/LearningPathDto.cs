namespace Source.DTOs
{
    // ==================== LEARNING PATH LEVEL ====================

    /// <summary>
    /// DTO cho Learning Path cơ bản (summary only)
    /// </summary>
    public class LearningPathDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CareerId { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public string CareerIconUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        // Progress summary (Subjects level only)
        public int TotalSubjects { get; set; }
        public int CompletedSubjects { get; set; }
        public double OverallProgress { get; set; } // 0-100
    }

    /// <summary>
    /// DTO cho chi tiết Learning Path (với Subjects list, không có Topics)
    /// </summary>
    public class LearningPathDetailDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CareerId { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public string CareerIconUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        // Progress summary
        public int TotalSubjects { get; set; }
        public int CompletedSubjects { get; set; }
        public double OverallProgress { get; set; }
        
        // Subjects list (summary only, không có Topics)
        public List<SubjectSummaryDto> Subjects { get; set; } = new();
    }

    /// <summary>
    /// DTO cho Subject summary (trong Learning Path detail)
    /// </summary>
    public class SubjectSummaryDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string Reason { get; set; } = string.Empty;
        
        // Progress summary (Topics level only)
        public int TotalTopics { get; set; }
        public int CompletedTopics { get; set; }
        public double SubjectProgress { get; set; } // 0-100
        public bool IsCompleted { get; set; }
        public bool IsInProgress { get; set; }
    }

    // ==================== SUBJECT LEVEL ====================

    /// <summary>
    /// DTO cho chi tiết Subject (với Topics list, không có Questions)
    /// </summary>
    public class SubjectDetailDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string Reason { get; set; } = string.Empty;
        
        // Progress summary
        public int TotalTopics { get; set; }
        public int CompletedTopics { get; set; }
        public double SubjectProgress { get; set; } // 0-100
        public bool IsCompleted { get; set; }
        public bool IsInProgress { get; set; }
        
        // Topics list (summary only, không có Questions)
        public List<TopicSummaryDto> Topics { get; set; } = new();
    }

    /// <summary>
    /// DTO cho Topic summary (trong Subject detail)
    /// </summary>
    public class TopicSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DifficultyLevel { get; set; } // 1-5
        
        // Progress summary (Questions level only)
        public int TotalQuestions { get; set; }
        public int CompletedQuestions { get; set; }
        public double TopicProgress { get; set; } // 0-100
        public bool IsCompleted { get; set; }
        public bool IsInProgress { get; set; }
        public DateTime? LastAccessedAt { get; set; }
    }

    // ==================== TOPIC LEVEL ====================

    /// <summary>
    /// DTO cho chi tiết Topic (và LearningQuestions)
    /// </summary>
    public class TopicDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DifficultyLevel { get; set; } // 1-5
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        
        // Progress summary
        public int TotalQuestions { get; set; }
        public int CompletedQuestions { get; set; }
        public double TopicProgress { get; set; } // 0-100
        public bool IsCompleted { get; set; }
        public bool IsInProgress { get; set; }
        public DateTime? LastAccessedAt { get; set; }
        
        // Learning Questions list
        public List<LearningQuestionDto> Questions { get; set; } = new();
    }

    /// <summary>
    /// DTO cho Learning Question
    /// </summary>
    public class LearningQuestionDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string Hint { get; set; } = string.Empty;
        public int Difficulty { get; set; }
        public int TopicId { get; set; }
        
        // Answers list
        public List<LearningAnswerDto> Answers { get; set; } = new();
        
        // User's answer (nếu đã làm)
        public int? UserAnswerId { get; set; }
        public bool? IsUserCorrect { get; set; }
    }

    /// <summary>
    /// DTO cho Learning Answer
    /// </summary>
    public class LearningAnswerDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public int LearningQuestionId { get; set; }
    }

    // ==================== REQUEST/RESPONSE DTOs ====================

    /// <summary>
    /// DTO cho request tạo Learning Path mới
    /// </summary>
    public class CreateLearningPathDto
    {
        public int CareerId { get; set; }
        public string? Title { get; set; } // Optional, sẽ default là Career Name nếu không cung cấp
    }

    /// <summary>
    /// DTO cho response khi tạo Learning Path thành công
    /// </summary>
    public class CreateLearningPathResponseDto
    {
        public int LearningPathId { get; set; }
        public string Message { get; set; } = string.Empty;
        public LearningPathDto LearningPath { get; set; } = null!;
    }

    /// <summary>
    /// DTO cho request submit answers cho Topic
    /// </summary>
    public class SubmitTopicAnswersDto
    {
        public int TopicId { get; set; }
        public int LearningPathId { get; set; }
        /// <summary>
        /// Danh sách các câu trả lời của user
        /// Key: QuestionId, Value: AnswerId
        /// </summary>
        public Dictionary<int, int> Answers { get; set; } = new();
    }

    /// <summary>
    /// DTO cho response khi submit answers cho Topic
    /// </summary>
    public class SubmitTopicAnswersResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double Score { get; set; } // 0-100
        public bool IsTopicCompleted { get; set; }
        public TopicSummaryDto TopicProgress { get; set; } = null!;
        public SubjectSummaryDto? SubjectProgress { get; set; } // Optional update
    }
}
