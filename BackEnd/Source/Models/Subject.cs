namespace Source.Models
{
    public class Subject
    {

        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;         // "LAP_TRINH_CO_BAN", "CAU_TRUC_DU_LIEU", "CO_SO_DU_LIEU" - Unique
        public string Name { get; set; } = string.Empty;           // "Nhập môn Lập trình", "OOP", "Database"
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "Core";                 // C
        public int SubjectIndex{get;set;}
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
        public ICollection<CareerSubject> CareerSubjects { get; set; } = new List<CareerSubject>();
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

    }
}
