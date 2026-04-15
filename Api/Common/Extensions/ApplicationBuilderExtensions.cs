using Api.Common.Middleware;

namespace Api.Common.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiExceptionMiddleware>();
        }
    }
}
