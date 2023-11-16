using SharedModels.Question;

namespace BrainBoxUI.Models
{
    public class SolveRunModel : BaseSolveRunModel
    {
        public SolveRunModel(List<int> questions) : base(questions)
        {
        }

        public QuestionDTO? currentQuestion { get; set; }
    }
}