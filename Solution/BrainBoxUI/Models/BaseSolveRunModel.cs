using SharedModels.Question;

namespace BrainBoxUI.Models
{
    public class BaseSolveRunModel
    {
        public BaseSolveRunModel(List<int> questions)
        {
            questionsId = questions;
            answers = new List<int>();
            currentQuestionIndex = 0;
        }

        public List<int> questionsId { get; set; }
        public List<int> answers { get; set; }
        public int currentQuestionIndex { get; set; }
    }
}
