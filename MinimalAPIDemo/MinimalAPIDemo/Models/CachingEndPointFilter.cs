using Microsoft.Extensions.Caching.Memory;
namespace MinimalAPIDemo.Models
{
    // This endpoint filter adds caching to Minimal API endpoints using in-memory cache.
    public class CachingEndPointFilter : IEndpointFilter
    {
        // Memory cache instance injected from DI container for storing cached data
        private readonly IMemoryCache _cache;

        // Duration for which the cached response should be stored
        private readonly TimeSpan _cacheDuration;

        // Constructor accepts the memory cache instance and an optional cache duration (defaults to 30 seconds)
        public CachingEndPointFilter(IMemoryCache cache, TimeSpan? cacheDuration = null)
        {
            _cache = cache;
            // Use provided cache duration or default to 30 seconds if null
            _cacheDuration = cacheDuration ?? TimeSpan.FromSeconds(30);
        }

        // This method intercepts endpoint execution to provide caching behavior
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // Get the HttpContext to access request details
            var httpContext = context.HttpContext;

            // Generate a unique cache key based on HTTP method, request path, and query string
            // Ensures different requests (e.g., different query params) cache separately
            var cacheKey = $"{httpContext.Request.Method}:{httpContext.Request.Path}{httpContext.Request.QueryString}";

            // Try to get the cached response from the cache using the cache key
            if (_cache.TryGetValue(cacheKey, out object? cachedResponse))
            {
                // If cached response exists, return it immediately without calling the endpoint handler
                return cachedResponse;
            }

            // If no cached response found, proceed to call the actual endpoint handler (or next filter)
            var result = await next(context);

            // If the result is an IResult (typically an API response), cache it.
            // You can customize this condition if you want more control over what gets cached.
            if (result is IResult)
            {
                // Store the result in memory cache with the computed cache key and expiration duration
                _cache.Set(cacheKey, result, _cacheDuration);
            }

            // Return the fresh result from the endpoint handler
            return result;
        }
    }
}