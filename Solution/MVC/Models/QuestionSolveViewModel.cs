using SharedModels.Question;

namespace MVC.Models
{
    public class QuestionSolveViewModel
    {
        public string QuestionName { get; set; }
        public string RoomName { get; set; }
        public QuestionModel QuestionModel { get; set; }
    }
}
