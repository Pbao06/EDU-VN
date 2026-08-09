using Source.Models;
namespace Source.DTOs.Tranning
{
    public class CountinueLearningPath
    {
        [Required(ErrorMessage=" Id khong duoc null")]
        public int LearningPathId{get;set;}
        public string CareerName{get;set;}
        public int Progress{get;set;}
        public string? CurrentSubject{get;set;}=string.Empty;
        public string? CurrrentTopic{get;set;}
        public int? CompletedTopic{get;set;}
        public int TotalTopic{get;set;}
    }
}

// Response
// {
//    "learningPathId":1,
//    "career":"Backend Developer",
//    "progress":67,
//    "currentSubject":"LINQ",
//    "currentTopic":"GroupBy",
//    "completedTopic":24,
//    "totalTopic":36
// }