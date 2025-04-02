using EComMSSharedLibrary.ComonResponseModel;
using System.Net;
using System.Text.Json;

namespace UserService.MiddleWares.Exception
{
    public class GlobalExceptionHandlerForUserService
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerForUserService> _logger;

        public GlobalExceptionHandlerForUserService(RequestDelegate next, ILogger<GlobalExceptionHandlerForUserService> logger)
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
            catch (System.Exception ex)
            {

                _logger.LogError(ex, "An unhandled exception occurred");


                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, System.Exception exception)
        {
            context.Response.ContentType = "application/json";


            var (statusCode, message) = GetStatusCodeAndMessage(exception);
            context.Response.StatusCode = (int)statusCode;


            var response = new ErrorResponse
            {
                Status = statusCode,
                Message = message,
               // TraceId = context.TraceIdentifier
            };


            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });


            await context.Response.WriteAsync(jsonResponse);
        }

        private static (HttpStatusCode statusCode, string message) GetStatusCodeAndMessage(System.Exception exception)
        {

            return exception switch
            {
                ArgumentException _ => (HttpStatusCode.BadRequest, "Invalid input provided"),
                KeyNotFoundException _ => (HttpStatusCode.NotFound, "The requested resource was not found"),
                UnauthorizedAccessException _ => (HttpStatusCode.Unauthorized, "You are not authorized to access this resource"),
                InvalidOperationException _ => (HttpStatusCode.BadRequest, "The requested operation is invalid"),
                TimeoutException _ => (HttpStatusCode.RequestTimeout, "The operation timed out"),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later")
            };
        }
    }

}
