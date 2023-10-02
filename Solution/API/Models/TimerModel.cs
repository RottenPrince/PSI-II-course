namespace API.Models
{
    public class TimerModel
    {
        public int Id { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsRunning
        {
            get; set;
        }
    }
}
