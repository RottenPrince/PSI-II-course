using SharedModels.Question;

namespace MVC.Models
{
    public class ReviewRunModel : BaseSolveRunModel
    {
        public ReviewRunModel(List<int> questions) : base(questions)
        {
        }

        public QuestionWithAnswerTransferModel? currentQuestion { get; set; }
        public int correctAnswersCount { get; set; }
    }
}