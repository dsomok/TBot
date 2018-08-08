using Microsoft.AspNetCore.Builder;

namespace TBot.Api.Middleware.Logging
{
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}