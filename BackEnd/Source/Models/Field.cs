namespace Source.Models
{
    /// <summary>
    /// Entity đại diện cho lĩnh vực (Field) - thay thế string tự do
    /// Ví dụ: CNTT, Marketing, Kinh tế, Y tế...
    /// </summary>
    public class Field
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;         // "CNTT", "MARKETING", "KINH_TE" - Unique, không thay đổi
        public string Name { get; set; } = string.Empty;           // "Công nghệ thông tin", "Marketing", "Kinh tế"
        public string Description { get; set; } = string.Empty;   // Mô tả chi tiết về lĩnh vực

        // Navigation Properties
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Career> Careers { get; set; } = new List<Career>();
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }
}