namespace API.Models
{
    public class APIResultWithData<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Result { get; set; }

        public static APIResultWithData<T> CreateSuccess(T data)
        {
            return new APIResultWithData<T>
            {
                Success = true,
                Message = "OK",
                Result = data
            };
        }

        public static APIResultWithData<T> CreateFailure (string message)
        {
            return new APIResultWithData<T>
            {
                Success = false,
                Message = message
            };
        }
    }
}
