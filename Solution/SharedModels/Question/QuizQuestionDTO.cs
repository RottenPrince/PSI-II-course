namespace SharedModels.Question
{
    public class QuizQuestionDTO
    {
        public QuestionWithAnswerDTO Question { get; set; }
        public int SelectedAnswerOption { get; set; }
    }
}
