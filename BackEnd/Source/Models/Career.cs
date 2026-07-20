namespace Source.Models
{
    public class Career
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;         // "FRONTEND_DEV", "BACKEND_DEV", "DATA_SCIENTIST" - Unique
        public string Name { get; set; } = string.Empty;           // "Lập trình viên Backend", "Data Scientist"
        public int FieldId { get; set; }                           // FK -> Field (thay thế string Field)
        public string Description { get; set; } = string.Empty;
        public string Responsibilities { get; set; } = string.Empty;
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public string DemandLevel { get; set; } = "High";          // High / Medium / Low
        public string IconUrl { get; set; } = string.Empty;
        public int PopularityScore { get; set; }                   // Để sort
        
        // New properties for frontend
        public int Difficulty { get; set; } = 3;                  // 1-5 scale for difficulty
        public string RequiredSkills { get; set; } = string.Empty; // JSON array or comma-separated
        public string Tags { get; set; } = string.Empty;           // JSON array or comma-separated

        // Navigation Properties
        public Field? Field { get; set; }                         // Navigation đến Field
        public ICollection<QuizCareerRecommendation> Recommendations { get; set; } = new List<QuizCareerRecommendation>();
        public ICollection<CareerSubject> CareerSubjects { get; set; } = new List<CareerSubject>();
        public ICollection<LearningPath> LearningPaths { get; set; } = new List<LearningPath>();
        public ICollection<AnswerCareerWeight> AnswerCareerWeights { get; set; } = new List<AnswerCareerWeight>();
    }
}
