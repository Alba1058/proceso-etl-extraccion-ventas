using Api.Common.Exceptions;
using System.Text.Json;

namespace Api.Common.Middleware
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                _logger.LogWarning(ex, "Error controlado en la API.");
                await WriteErrorAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado en la API.");
                await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "Ocurrio un error interno en la API.");
            }
        }

        private static Task WriteErrorAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var payload = JsonSerializer.Serialize(new
            {
                success = false,
                message,
                statusCode
            });

            return context.Response.WriteAsync(payload);
        }
    }
}
