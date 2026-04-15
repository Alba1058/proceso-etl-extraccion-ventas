namespace Api.Common.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(string message, int statusCode = StatusCodes.Status500InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
