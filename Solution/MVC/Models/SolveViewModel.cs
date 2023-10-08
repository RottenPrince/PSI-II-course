using SharedModels.Question;

namespace MVC.Models
{
    public class SolveViewModel
    {
        public SolveViewModel(QuestionModel questionModel, string questionName, string room)
        {
            Question = questionModel;
            QuestionName = questionName;
            Room = room;
        }
        public QuestionModel? Question { get; set; }
        public string? QuestionName { get; set; }
        public string? Room { get; set; }
    }
}
