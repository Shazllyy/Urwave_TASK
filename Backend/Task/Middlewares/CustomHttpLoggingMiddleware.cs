public class CustomHttpLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomHttpLoggingMiddleware> _logger;

    public CustomHttpLoggingMiddleware(RequestDelegate next, ILogger<CustomHttpLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;

        // Log the request
        _logger.LogInformation($"Request: {request.Method} {request.Path}");

        // Hook into the response body to log it only if it's text
        var originalResponseBodyStream = response.Body;
        using (var responseBodyStream = new MemoryStream())
        {
            response.Body = responseBodyStream;

            await _next(context);  // Process the request

            // Only log if the response is text-based (you can modify this as needed)
            if (response.ContentType?.StartsWith("text") == true || response.ContentType?.StartsWith("application/json") == true)
            {
                // Log the response body as text
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBodyText = new StreamReader(responseBodyStream).ReadToEnd();
                _logger.LogInformation($"Response: {response.StatusCode} - {responseBodyText}");
            }

            // Copy the response body to the original stream
            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        }
    }
}
