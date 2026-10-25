using UserAPI.Application.Common.Exceptions;
using UserAPI.Models;

namespace UserAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                    throw;

                var (statusCode, response) = ToErrorResponse(ex);

                if (statusCode >= StatusCodes.Status500InternalServerError)
                    _logger.LogError(ex, "Unhandled error processing {Method} {Path}", context.Request.Method, context.Request.Path);
                else
                    _logger.LogInformation("Request {Method} {Path} failed: {Message}", context.Request.Method, context.Request.Path, ex.Message);

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(response);
            }
        }

        public static (int StatusCode, ErrorResponse Response) ToErrorResponse(Exception exception) => exception switch
        {
            AppException appException => (ToStatusCode(appException.Type), new ErrorResponse(appException.Message, appException.Code)),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, new ErrorResponse(exception.Message)),
            _ => (StatusCodes.Status500InternalServerError, new ErrorResponse(exception.Message))
        };

        private static int ToStatusCode(ErrorType type) => type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
