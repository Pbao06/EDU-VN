namespace Source.DTOs.Admin
{
    public class AdminCareerDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Responsibilities { get; set; } = string.Empty;
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public string DemandLevel { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public int PopularityScore { get; set; }
    }

    public class CreateCareerDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int FieldId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Responsibilities { get; set; } = string.Empty;
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public string DemandLevel { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public int PopularityScore { get; set; }
    }

    public class UpdateCareerDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int FieldId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Responsibilities { get; set; } = string.Empty;
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public string DemandLevel { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public int PopularityScore { get; set; }
    }
}