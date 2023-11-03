namespace SharedModels.Question
{
    public record class QuestionLocationTransferModel
    {
        public int QuestionId { get; init; }
        public int RoomId { get; init; }
    }
}
