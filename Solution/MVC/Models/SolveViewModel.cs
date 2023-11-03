using SharedModels.Question;

namespace MVC.Models
{
    public class SolveViewModel
    {
        public SolveViewModel(QuestionTransferModel questionModel, string questionName)
        {
            Question = questionModel;
            QuestionName = questionName;
        }
        public QuestionTransferModel? Question { get; set; }
        public string? QuestionName { get; set; }
    }
}
