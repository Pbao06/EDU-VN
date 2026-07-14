namespace Source.Models
{
    public class LearningPath
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public int CareerId { get; set; }
        public Career Career { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; } // Khi user hoàn thành tất cả subjects trong learning path
    }
}
