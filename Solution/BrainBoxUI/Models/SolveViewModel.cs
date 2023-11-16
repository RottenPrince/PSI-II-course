using SharedModels.Question;

namespace BrainBoxUI.Models
{
    public class SolveViewModel
    {
        public SolveViewModel(QuestionDTO questionModel, int questionId)
        {
            Question = questionModel;
            QuestionId = questionId;
        }
        public QuestionDTO? Question { get; set; }
        public int? QuestionId { get; set; }
    }
}
