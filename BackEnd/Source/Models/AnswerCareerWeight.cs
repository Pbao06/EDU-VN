namespace Source.Models
{
    /// <summary>
    /// Entity cấu hình tĩnh quy định mỗi đáp án cộng/trừ điểm cho Career nào
    /// Đây là bảng cấu hình để tính điểm rule-based cho career recommendation
    /// Chỉ dùng cho RecommendationAnswer
    /// </summary>
    public class AnswerCareerWeight
    {
        public int Id { get; set; }
        public int RecommendationAnswerId { get; set; }  // FK → RecommendationAnswer
        public int CareerId { get; set; }  // FK → Career
        public int Weight { get; set; }  // Trọng số: -3 đến +5 (có thể âm hoặc dương)

        // Navigation Properties
        public RecommendationAnswer RecommendationAnswer { get; set; } = null!;
        public Career Career { get; set; } = null!;
    }
}