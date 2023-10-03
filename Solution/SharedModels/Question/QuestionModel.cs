namespace SharedModels.Question
{
    public class QuestionModel
    {
        public QuestionModel()
        {
            AnswerOptions = new List<string>();
        }

        public string? Question { get; set; }
        public List<string>? AnswerOptions { get; set; }
        public string? ImageName { get; set; }
    }
}
