using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Source.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                // log exception before writing the response 
                LogException(context,exception);
                await HandleExceptionAsync(context, exception);
            }
        }
        private void LogException(HttpContext context,Exception exception)
        {
            // check to else error was config is 500 
            if(exception is not (BadRequestException or UnauthorizedException or ForbiddenException or NotFoundException))
            {
                _logger.LogError(exception, "An unhandled exception orcurred while processing request {Path}. Error: {Message}"
            ,context.Request.Path,exception.Message);
            }
            else
            {
                // Client-side errors (4xx) usually don't need Error-level logging, Warning or Information is cleaner
                _logger.LogWarning(exception, "A client error occurred while processing request {Path}: {Message}",
                    context.Request.Path, exception.Message);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var errorResponse = new ExceptionError(exception.Message);
            var jsonResponse = JsonSerializer.Serialize(errorResponse);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
