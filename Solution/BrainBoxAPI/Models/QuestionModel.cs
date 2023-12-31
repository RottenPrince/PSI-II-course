namespace BrainBoxAPI.Models
{
    public class QuestionModel
    {
        public QuestionModel()
        {
            AnswerOptions = new List<AnswerOptionModel>();
        }
        public int Id { get; set; }

        public string Title { get; set; }

        public string? ImageSource { get; set; }

        public int RoomId { get; set; }
        public RoomModel Room { get; set; }

        public List<AnswerOptionModel> AnswerOptions { get; set; }
        public List<QuizQuestionRelationModel> QuizRelations { get; set; }
    }
}
