namespace Source.Models
{
    public class UserProgress
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public int TopicId { get; set; }
        public Topic Topic { get; set; } = null!;
        public int CompletionPercentage { get; set; } = 0;
        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
    }
}
