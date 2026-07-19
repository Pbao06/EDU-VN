namespace Source.DTOs
{
    public class UserAnswerDto
    {
        public int QuestionId { get; set; } // id cua question
        public string ContentQuestion { get; set; } = string.Empty;
        public int AnswerId { get; set; }
        public string ContentAnswer {  get; set; }=string.Empty;

    }
    public class ListUserAnswerDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public ICollection<UserAnswerDto>? UserAnswers { get; set; } // neu khong co thi return lai list null cung dc 
  

    }
}
