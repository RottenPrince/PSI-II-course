namespace BrainBoxAPI.Models
{
    public class SolveRunModel
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }

        public RoomModel Room { get; set; }
        public List<QuestionSolveRunJoinModel> SolveRunJoin { get; set; }
    }
}
