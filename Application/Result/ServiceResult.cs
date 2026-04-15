namespace Application.Result
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;

        public static ServiceResult Success(string message) => new()
        {
            IsSuccess = true,
            Message = message
        };

        public static ServiceResult Failure(string message) => new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}
