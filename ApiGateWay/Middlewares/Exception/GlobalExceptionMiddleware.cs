using EComMSSharedLibrary.ComonResponseModel;
using System.Net;
using System.Text.Json;

namespace ApiGateWay.Middlewares.Exception
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (!context.Response.HasStarted)
                {
                    // Check if ModelState is valid
                    var modelState = context.RequestServices.GetService(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinderFactory)) as Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinderFactory;

                    if (modelState == null)
                    {
                        // If model validation is unsuccessful, return custom error response
                        var errors = context.Items
                            .Where(x => x.Value is string)
                            .Select(x => x.Value.ToString()).ToList();

                        var response = ApiResponse<string>.Create(
                            null,  // No data in case of validation failure
                            "Validation failed", // Message
                            StatusCodes.Status400BadRequest, // HTTP status code
                            errors // List of error messages
                        );

                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(response);
                        return;
                    }
                }   
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
