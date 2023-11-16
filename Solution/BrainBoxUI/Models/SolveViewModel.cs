using SharedModels.Question;

namespace BrainBoxUI.Models
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
