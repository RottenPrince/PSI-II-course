namespace SharedModels.Question
{
    public record class QuestionLocationModel
    {
        public QuestionLocationModel(string name, string roomId)
        {
            Name = name;
            RoomId = roomId;
        }
        public string Name { get; init; }
        public string RoomId { get; init; }
    }
}
