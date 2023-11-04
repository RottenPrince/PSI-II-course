using SharedModels.Question;

namespace MVC.Models
{
    public class SolveResultViewModel
    {
        public SolveResultViewModel(QuestionWithAnswerTransferModel questionModel, int questionId, string roomId, int? wrongAnswerIndex = null)
        {
            Question = questionModel;
            QuestionId = questionId;
            RoomId = roomId;
            WrongAnswerIndex = wrongAnswerIndex;
        }
        public QuestionWithAnswerTransferModel Question { get; set; }
        public int? QuestionId { get; set; }
        public string? RoomId { get; set; }
        public int? WrongAnswerIndex { get; set; }
    }
}
