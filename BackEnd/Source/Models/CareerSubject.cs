namespace Source.Models
{
    public class CareerSubject
    {
        public int Id { get; set; }
        public int CareerId { get; set; }
        public Career Career { get; set; } = null!;
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
        public int Priority { get; set; }           // 1 = Quan trọng nhất, 2 = Quan trọng, 3 = Hỗ trợ
        public string Reason { get; set; } = string.Empty;   // "Cần thiết để hiểu logic code"
    }
}

