namespace SharedModels.Lobby
{
    public struct RoomContentStruct
    {
        public RoomContentStruct(int questionAmount, string roomName)
        {
            QuestionAmount = questionAmount;
            RoomName = roomName;
        }
        public int QuestionAmount { get; set; }
        public string RoomName { get; set; }
    }
}
