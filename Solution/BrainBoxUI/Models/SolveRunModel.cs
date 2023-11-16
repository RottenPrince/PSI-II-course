using SharedModels.Question;

namespace BrainBoxUI.Models
{
    public class SolveRunModel : BaseSolveRunModel
    {
        public SolveRunModel(List<int> questions) : base(questions)
        {
        }

        public QuestionTransferModel? currentQuestion { get; set; }
    }
}