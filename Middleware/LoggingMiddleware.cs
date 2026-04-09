using Serilog;

namespace TaskFlow.Api.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var start = DateTime.UtcNow;

            await _next(context);

            var duration = DateTime.UtcNow - start;

            Log.Information(
                "Request {Method} {Path} responded {StatusCode} in {Duration} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration.TotalMilliseconds
            );
        }
    }
}
