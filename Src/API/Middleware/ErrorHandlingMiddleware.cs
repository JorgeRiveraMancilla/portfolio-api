using System.Net;
using System.Text.Json;
using Application.Common;
using FluentValidation;

namespace API.Middleware
{
    public class ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger
    )
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode;
            ApiResponse<object> apiResponse;

            switch (exception)
            {
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    apiResponse = ApiResponse<object>.ErrorResponse(
                        "Validation failed",
                        [.. validationException.Errors.Select(e => e.ErrorMessage)]
                    );
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    apiResponse = ApiResponse<object>.ErrorResponse(
                        "An error occurred while processing your request"
                    );
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(apiResponse, _jsonOptions));
        }
    }
}
