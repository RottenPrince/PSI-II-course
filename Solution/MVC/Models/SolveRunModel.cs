using SharedModels.Question;

namespace MVC.Models
{
    public class SolveRunModel : BaseSolveRunModel
    {
        public SolveRunModel(List<int> questions) : base(questions)
        {
        }

        public QuestionTransferModel? currentQuestion { get; set; }
    }
}