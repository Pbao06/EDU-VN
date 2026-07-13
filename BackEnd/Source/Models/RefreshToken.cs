namespace Source.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? DeviceInfo { get; set; }
        public bool IsRevoked => RevokedAt != null;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        // Navigation Property
        public User User { get; set; } = null!;
    }
}