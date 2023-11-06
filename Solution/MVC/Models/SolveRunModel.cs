using SharedModels.Question;

namespace MVC.Models
{
    public class SolveRunModel
    {
        public SolveRunModel(List<int> questions) {
            questionsId = questions;
            answers = new List<int>();
            currentQuestionIndex = 0;
        }

        public List<int> questionsId { get; set; }
        public List<int> answers { get;set; }
        public int currentQuestionIndex {  get; set; }

        public QuestionTransferModel? currentQuestion { get; set; }
    }
}
