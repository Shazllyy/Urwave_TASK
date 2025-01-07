using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        // Constructor to initialize dependencies
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Correct async signature for middleware
        public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)  // Return Task
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                await HandleExceptionAsync(context, ex); 
            }
        }

        private async System.Threading.Tasks.Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            // The error response structure
            var response = new
            {
                message = "An error occurred while processing your request.",
                detail = exception.Message
            };

            // Write the response to the HTTP context
            await context.Response.WriteAsJsonAsync(response); // Correctly await the asynchronous response
        }
    }
}
