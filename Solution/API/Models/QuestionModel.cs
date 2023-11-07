namespace API.Models
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
        public int SolveRunJoinID { get; set; }
        public List<SolveRunModel> SolveRuns { get; set; }
        public List<QuestionSolveRunJoinModel> Joins { get; set; }
    }
}
