using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using AutoMapper;
using FluentValidation;
using MuseumSystem.Application.Exceptions;
using StackExchange.Redis;

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
                    AuthenticationException => 401,
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    ValidationException => (int)HttpStatusCode.BadRequest,
                    ArgumentException => (int)HttpStatusCode.BadRequest,
                    InvalidOperationException => (int)HttpStatusCode.BadRequest,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    TimeoutException => (int)HttpStatusCode.RequestTimeout,
                    NotImplementedException => (int)HttpStatusCode.NotImplemented,
                    FormatException => (int)HttpStatusCode.BadRequest,
                    OverflowException => (int)HttpStatusCode.BadRequest,
                    DivideByZeroException => (int)HttpStatusCode.BadRequest,
                    IndexOutOfRangeException => (int)HttpStatusCode.BadRequest,
                    NullReferenceException => (int)HttpStatusCode.BadRequest,
                    InvalidCastException => (int)HttpStatusCode.BadRequest,
                    StackOverflowException => (int)HttpStatusCode.InternalServerError,
                    OutOfMemoryException => (int)HttpStatusCode.InternalServerError,
                    NotSupportedException => (int)HttpStatusCode.BadRequest,
                    OperationCanceledException => (int)HttpStatusCode.RequestTimeout,
                    JsonException => (int)HttpStatusCode.BadRequest,
                    FileNotFoundException => (int)HttpStatusCode.NotFound,
                    DirectoryNotFoundException => (int)HttpStatusCode.NotFound,
                    PathTooLongException => (int)HttpStatusCode.BadRequest,
                    IOException => (int)HttpStatusCode.InternalServerError,

                    // AutoMapper specific exceptions
                    AutoMapperMappingException autoMapperMappingException => autoMapperMappingException.InnerException switch
                    {
                        ArgumentException => (int)HttpStatusCode.BadRequest,
                        KeyNotFoundException => (int)HttpStatusCode.NotFound,
                        _ => (int)HttpStatusCode.InternalServerError
                    },

                    // Redis specific exceptions
                    RedisConnectionException redisConnectionException => redisConnectionException.InnerException switch
                    {
                        TimeoutException => (int)HttpStatusCode.RequestTimeout,
                        _ => (int)HttpStatusCode.InternalServerError
                    },

                    //----- Custom application exceptions ------//
                    ConflictException => (int)HttpStatusCode.Conflict,
                    NotFoundException => (int)HttpStatusCode.NotFound,
                    InvalidAccessException => (int)HttpStatusCode.Forbidden,
                    ObjectDeletedException => (int)HttpStatusCode.BadRequest,

                    _ => (int)HttpStatusCode.InternalServerError
                };

                object response;

                if (ex is ValidationException ve)
                {
                    var errors = ve.Errors.Select(e => e.ErrorMessage).ToArray();
                    response = new
                    {
                        StatusCode = context.Response.StatusCode,
                        IsSuccess = false,
                        Message = errors
                    };
                }
                else
                {
                    response = new
                    {
                        StatusCode = context.Response.StatusCode,
                        IsSuccess = false,
                        Message = ex.Message,
                    };
                }

                var result = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(result);
            }
        }
    }
}
