namespace SharedModels.Lobby
{
    public record class RoomContentModel
    {
        public RoomContentModel(int questionAmount, string roomName)
        {
            QuestionAmount = questionAmount;
            RoomName = roomName;
        }
        public int QuestionAmount { get; init; }
        public string RoomName { get; init; }
    }
}
