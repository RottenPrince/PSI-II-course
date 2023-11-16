namespace BrainBoxAPI.Models
{
    public class RoomModel
    {
        public RoomModel()
        {
            Questions = new List<QuestionModel>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public List<QuestionModel> Questions { get; set; }
        public List<SolveRunModel> SolveRuns { get; set; }
    }
}
