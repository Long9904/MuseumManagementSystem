using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using FluentValidation;

namespace MuseumSystem.Api.Middleware
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
                _logger.LogError(ex, "Unhandled exception occurred.");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    AuthenticationException => (int)HttpStatusCode.Unauthorized,
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    ValidationException => (int)HttpStatusCode.BadRequest,
                    ArgumentException => (int)HttpStatusCode.BadRequest,
                    InvalidOperationException => (int)HttpStatusCode.BadRequest,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    TimeoutException => (int)HttpStatusCode.RequestTimeout,
                    NotImplementedException => (int)HttpStatusCode.NotImplemented,
                    FormatException => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                object response;

                if (ex is ValidationException ve)
                {
                    var errors = ve.Errors.Select(e => e.ErrorMessage).ToArray();
                    response = new
                    {
                        IsSuccess = false,
                        Errors = errors,
                        StatusCode = context.Response.StatusCode
                    };
                }
                else
                {
                    response = new
                    {
                        IsSuccess = false,
                        Errors = ex.Message,
                        StatusCode = context.Response.StatusCode
                    };
                }

                var result = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(result);
            }
        }
    }
}
