using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Serilog;
using Serilog.Context;

namespace TBot.Api.Middleware.Logging
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;


        public LoggingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("RequestID", context.TraceIdentifier))
            {
                await this.LogRequest(context.TraceIdentifier, context.Request);

                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    await this.LogResponse(context.TraceIdentifier, context.Response);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
        }


        private async Task LogRequest(string traceId, HttpRequest request)
        {
            var body = request.Body;
            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body = body;

            this._logger.Information(
                $"[{{RequestID}}] [{{Scheme}}] {{Uri}} {Environment.NewLine}QueryString: {{QueryString}}{Environment.NewLine}Body: {{Body}}",
                traceId,
                request.Scheme,
                $"{request.Host}{request.Path}",
                request.QueryString,
                bodyAsText
            );
        }

        private async Task LogResponse(string traceId, HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            this._logger.Information(
                "[{RequestID}] Response: {Response}",
                traceId,
                responseBody
            );
        }
    }
}
