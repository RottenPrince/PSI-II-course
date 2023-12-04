using BrainBoxAPI.Data;

namespace BrainBoxAPI.Models
{
    public class QuizModel
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }

        public RoomModel Room { get; set; }
        public List<QuizQuestionRelationModel> QuestionRelations { get; set; }

        public string? UserId { get; set; } = "";
        public ApplicationUser User { get; set; } = new ApplicationUser();
    }
}
