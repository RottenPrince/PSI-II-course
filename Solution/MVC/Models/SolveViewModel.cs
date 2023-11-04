using SharedModels.Question;

namespace MVC.Models
{
    public class SolveViewModel
    {
        public SolveViewModel(QuestionTransferModel questionModel, int questionId)
        {
            Question = questionModel;
            QuestionId = questionId;
        }
        public QuestionTransferModel? Question { get; set; }
        public int? QuestionId { get; set; }
    }
}
