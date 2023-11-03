using SharedModels.Question;

namespace MVC.Models
{
    public class SolveResultViewModel
    {
        public SolveResultViewModel(QuestionWithAnswerTransferModel questionModel, string questionName, string roomId, int? wrongAnswerIndex = null)
        {
            Question = questionModel;
            QuestionName = questionName;
            RoomId = roomId;
            WrongAnswerIndex = wrongAnswerIndex;
        }
        public QuestionWithAnswerTransferModel Question { get; set; }
        public string? QuestionName { get; set; }
        public string? RoomId { get; set; }
        public int? WrongAnswerIndex { get; set; }
    }
}
