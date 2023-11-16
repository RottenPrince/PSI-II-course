namespace API.Models
{
    public class QuestionSolveRunJoinModel
    {
        public int Id { get; set; }
        public int SolveRunModelID { get; set; }
        public SolveRunModel SolveRun;
        public int QuestionModelID { get; set; }
        public QuestionModel Question;
        public int? AnswerOptionModelID { get; set; }
        public AnswerOptionModel? SelectedAnswerOption;
    }
}
