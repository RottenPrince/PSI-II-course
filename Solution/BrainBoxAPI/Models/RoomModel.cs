using BrainBoxAPI.Data;

namespace BrainBoxAPI.Models
{
    public class RoomModel
    {
        public RoomModel()
        {
            Questions = new List<QuestionModel>();
            Users = new List<ApplicationUser>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string UniqueCode { get; set; }

        public List<QuestionModel> Questions { get; set; }
        public List<QuizModel> Quizs { get; set; }

        public List<ApplicationUser> Users { get; set; }
    }
}
