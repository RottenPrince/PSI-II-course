namespace SharedModels.Question
{
    public class QuestionLocationModel
    {
        public QuestionLocationModel(string name, string roomId)
        {
            Name = name;
            RoomId = roomId;
        }
        public string Name { get; set; }
        public string RoomId { get; set; }
    }
}
