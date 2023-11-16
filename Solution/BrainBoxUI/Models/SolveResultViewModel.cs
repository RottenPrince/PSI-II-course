using SharedModels.Question;

namespace BrainBoxUI.Models
{
    public class SolveResultViewModel
    {
        public SolveResultViewModel(QuestionWithAnswerDTO questionModel, int questionId, string roomId, int? wrongAnswerIndex = null)
        {
            Question = questionModel;
            QuestionId = questionId;
            RoomId = roomId;
            WrongAnswerIndex = wrongAnswerIndex;
        }
        public QuestionWithAnswerDTO Question { get; set; }
        public int? QuestionId { get; set; }
        public string? RoomId { get; set; }
        public int? WrongAnswerIndex { get; set; }
    }
}
