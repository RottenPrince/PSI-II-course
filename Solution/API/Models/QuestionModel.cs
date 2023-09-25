namespace API.Models
{
    public class QuestionModel
    {
        public string? Question { get; set; }
        public List<string>? AnswerOptions { get; set; }
        public string? ImageName { get; set; }
        public int CorrectAnswer { get; set; }
    }
}
