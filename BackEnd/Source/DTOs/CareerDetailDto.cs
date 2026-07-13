using System.ComponentModel.DataAnnotations;

namespace Source.DTOs
{
    public class CareerDetailDto
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage ="Name Required not null")]
        public required string Name { get; set; }           // "Lập trình viên Backend", "Data Scientist"
        public string Description { get; set; } = string.Empty;
        public string Responsibilities { get; set; } = string.Empty;
        public decimal MinSalary { get; set; }
        [Required(ErrorMessage =" Max Salary Required not null")]
        public decimal MaxSalary { get; set; }
        public string DemandLevel { get; set; } = "High";          // High / Medium / Low
        public string? IconUrl { get; set; } = string.Empty;
    }
    public class ListCareerDto
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage =" Required Name not null")]
        public required string Name { get; set; }
        public string? ShortDescription { get; set; }
        public decimal Salary { get; set; }
        public string? IconUrl { get; set; } = string.Empty;
        public string? DemandLevel { get; set; } = "High";
    }
}
