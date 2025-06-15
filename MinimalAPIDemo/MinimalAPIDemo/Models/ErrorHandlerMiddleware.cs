using System.Net;
using System.Text.Json;

namespace MinimalAPIDemo.Models
{
    public class ErrorHandlerMiddleware
    {
        // Holds the next middleware in the pipeline to invoke
        private readonly RequestDelegate _next;
        // Logger instance for logging errors
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        // Constructor injects the next middleware and a logger
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;     // Assign the next middleware in the chain
            _logger = logger; // Assign the injected logger
        }
        // This method is called for every HTTP request. Handles errors during the request processing.
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Invoke the next middleware in the pipeline; await in case it's asynchronous
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception with its stack trace and a message
                _logger.LogError(ex, "An unhandled exception has occurred.");
                // Set response content type as JSON because we will return a JSON error response
                context.Response.ContentType = "application/json";
                // Set HTTP status code to 500 (Internal Server Error)
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                // Create an anonymous object representing the error details
                var response = new
                {
                    Title = "An unexpected error occurred.",  // User-friendly Short title of the error
                    Status = context.Response.StatusCode,     // Include HTTP status code (500)
                    // Conditionally include detailed error message if environment is Development
                    Detail = context.RequestServices.GetService(typeof(IWebHostEnvironment)) is IWebHostEnvironment env && env.IsDevelopment()
                             ? ex.Message                    // Show detailed exception message in Development
                             : "Please contact support."     // Generic message in Production or other environments
                };
                // Serialize the anonymous error object into a JSON string
                var jsonResponse = JsonSerializer.Serialize(response);
                // Write the JSON error response to the HTTP response body asynchronously
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
