namespace SharedModels.Question
{
    public class SolveDisplayModel
    {
        public SolveDisplayModel(QuestionModel questionModel, string questionName, string room)
        {
            Question = questionModel;
            QuestionName = questionName;
            Room = room;
        }
        public QuestionModel? Question { get; set; }
        public string? QuestionName { get; set; }
        public string? Room { get; set; }
    }
}
