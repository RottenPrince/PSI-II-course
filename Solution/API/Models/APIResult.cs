namespace API.Models
{
    public class APIResult 
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public static APIResult CreateSuccess()
        {
            return new APIResult
            {
                Success = true,
                Message = "OK",
            };
        }

        public static APIResult CreateFailure (string message)
        {
            return new APIResult
            {
                Success = false,
                Message = message
            };
        }
    }
}
