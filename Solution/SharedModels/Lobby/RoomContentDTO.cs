namespace SharedModels.Lobby
{
    public record class RoomContentDTO
    {
        public int QuestionAmount { get; init; }
        public string RoomName { get; init; }
        public string UniqueCode { get; init; }
    }
}
