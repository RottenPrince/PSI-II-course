namespace SharedModels.Lobby
{
    public record struct RoomContentDTO
    {
        public RoomContentDTO(int questionAmount, string roomName)
        {
            QuestionAmount = questionAmount;
            RoomName = roomName;
        }
        public int QuestionAmount { get; init; }
        public string RoomName { get; init; }
    }
}
