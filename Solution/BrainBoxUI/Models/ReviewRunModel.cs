using SharedModels.Question;

namespace BrainBoxUI.Models
{
    public class ReviewRunModel : BaseSolveRunModel
    {
        public ReviewRunModel(List<int> questions) : base(questions)
        {
        }

        public QuestionWithAnswerDTO? currentQuestion { get; set; }
        public int correctAnswersCount { get; set; }
    }
}