namespace BrainBoxAPI.Models
{
    public class QuizQuestionRelationModel
    {
        public int Id { get; set; }
        public int QuizModelID { get; set; }
        public QuizModel Quiz;
        public int QuestionModelID { get; set; }
        public QuestionModel Question;
        public int? AnswerOptionModelID { get; set; }
        public AnswerOptionModel? SelectedAnswerOption;
    }
}
