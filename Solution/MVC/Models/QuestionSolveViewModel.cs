using SharedModels.Question;

namespace MVC.Models
{
    public class QuestionSolveViewModel
    {
        public string QuestionName { get; set; }
        public string RoomId { get; set; }
        public QuestionModel QuestionModel { get; set; }
    }
}
