namespace Source.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;  // "Hàm số bậc 2", "Grammar Tenses"
        public string Description { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
        public int DifficultyLevel { get; set; }  // 1-5
        public ICollection<UserProgress> UserProgresses = new HashSet<UserProgress>();
        public ICollection<LearningQuestion> Questions = new HashSet<LearningQuestion>();
    }
}
