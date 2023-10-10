using SharedModels.Question;

namespace MVC.Models
{
    public class SolveResultViewModel
    {
        public SolveResultViewModel(QuestionModelWithAnswer questionModel, string questionName, string roomId, int selectedOption)
        {
            Question = questionModel;
            QuestionName = questionName;
            RoomId = roomId;
            SelectedOption = selectedOption;
        }
        public QuestionModelWithAnswer Question { get; set; }
        public string? QuestionName { get; set; }
        public string? RoomId { get; set; }
        public int SelectedOption { get; set; }
    }
}
