namespace SharedModels.Lobby
{
    public class RoomContentModel
    {
        public RoomContentModel(int questionAmount, string roomName)
        {
            QuestionAmount = questionAmount;
            RoomName = roomName;
        }
        public int QuestionAmount { get; set; }
        public string RoomName { get; set; }
    }
}
