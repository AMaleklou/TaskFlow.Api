using System.Net;
using System.Text.Json;
using TaskFlow.Api.Common;
using TaskFlow.Api.Common.Exceptions;
using Serilog;

namespace TaskFlow.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            object response;

            switch (ex)
            {
                case NotFoundException notFoundEx:
                    statusCode = (int)HttpStatusCode.NotFound;
                    response = ApiResponse.Fail<string>(notFoundEx.Message);
                    break;

                case BadRequestException badRequestEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = ApiResponse.Fail<string>(badRequestEx.Message);
                    break;

                case ValidationException validationEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        success = false,
                        message = validationEx.Message,
                        errors = validationEx.Errors
                    };
                    break;

                default:
                    response = ApiResponse.Fail<string>("An unexpected error occurred");
                    break;
            }

            context.Response.StatusCode = statusCode;
            var json = JsonSerializer.Serialize(response);
         
            return context.Response.WriteAsync(json);
        }
    }
}
