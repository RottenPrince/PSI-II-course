namespace API.Models
{
    public class RoomModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<QuestionModel> Questions { get; set; }
    }
}
