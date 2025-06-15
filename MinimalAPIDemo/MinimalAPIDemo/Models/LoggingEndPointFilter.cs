using System.Diagnostics;
namespace MinimalAPIDemo.Models
{
    // This filter logs request details and measures the execution time for Minimal API endpoints.
    public class LoggingEndPointFilter : IEndpointFilter
    {
        // Logger instance injected via constructor, used to write log messages
        private readonly ILogger<LoggingEndPointFilter> _logger;

        // Constructor receives an ILogger from dependency injection
        public LoggingEndPointFilter(ILogger<LoggingEndPointFilter> logger)
        {
            _logger = logger;
        }

        // This method is called for each request that the filter is applied to
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // Start a stopwatch to measure how long the endpoint takes to execute.
            var stopwatch = Stopwatch.StartNew();

            // Log the beginning of the request, including HTTP method and endpoint path.
            _logger.LogInformation($"Starting request {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");

            // Call the next delegate in the pipeline (next filter or endpoint handler)
            var result = await next(context);

            // Stop the stopwatch since endpoint processing is done.
            stopwatch.Stop();

            // Log the completion of the request, including HTTP method, path, and elapsed time in milliseconds.
            _logger.LogInformation($"Finished request {context.HttpContext.Request.Method} {context.HttpContext.Request.Path} in {stopwatch.ElapsedMilliseconds} ms");

            // Return the result from the endpoint (or the next filter).
            return result;
        }
    }
}