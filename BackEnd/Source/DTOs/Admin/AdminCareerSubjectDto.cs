namespace Source.DTOs.Admin
{
    public class AdminCareerSubjectDto
    {
        public int CareerId { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class CreateCareerSubjectDto
    {
        public int CareerId { get; set; }
        public int SubjectId { get; set; }
        public int Priority { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class UpdateCareerSubjectDto
    {
        public int CareerId { get; set; }
        public int SubjectId { get; set; }
        public int Priority { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}