namespace Source.DTOs.Admin
{
    public class AdminAnswerCareerWeightDto
    {
        public int Id { get; set; }
        public int RecommendationAnswerId { get; set; }
        public string AnswerContent { get; set; } = string.Empty;
        public int CareerId { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public int Weight { get; set; }
    }

    public class CreateAnswerCareerWeightDto
    {
        public int RecommendationAnswerId { get; set; }
        public int CareerId { get; set; }
        public int Weight { get; set; }
    }

    public class UpdateAnswerCareerWeightDto
    {
        public int RecommendationAnswerId { get; set; }
        public int CareerId { get; set; }
        public int Weight { get; set; }
    }
}