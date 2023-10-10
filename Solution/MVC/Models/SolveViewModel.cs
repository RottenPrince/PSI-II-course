using SharedModels.Question;

namespace MVC.Models
{
    public class SolveViewModel
    {
        public SolveViewModel(QuestionModel questionModel, string questionName)
        {
            Question = questionModel;
            QuestionName = questionName;
        }
        public QuestionModel? Question { get; set; }
        public string? QuestionName { get; set; }
    }
}
