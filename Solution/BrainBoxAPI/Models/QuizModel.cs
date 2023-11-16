namespace BrainBoxAPI.Models
{
    public class QuizModel
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }

        public RoomModel Room { get; set; }
        public List<QuizQuestionRelationModel> QuestionRelations { get; set; }
    }
}
