namespace BrainBoxAPI.Models
{
    public class AnswerOptionModel
    {
        public int Id { get; set; }

        public string OptionText { get; set; }

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public QuestionModel Question { get; set; }
    }
}
