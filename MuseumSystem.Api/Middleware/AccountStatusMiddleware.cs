using System.Security.Claims;
using System.Text.Json;
using MuseumSystem.Domain.Abstractions;

namespace MuseumSystem.Api.Middleware
{
    public class AccountStatusMiddleware
    {
        private readonly RequestDelegate _next;
        public AccountStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
        {
            var user = context.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                                ?? throw new UnauthorizedAccessException("User is not login");
                var account = await unitOfWork.AccountRepository.GetByIdAsync(userId);
                var status = account?.Status.ToString();

                var path = context.Request.Path.Value?.ToLower();
                if (status == "Pending" && !IsPublicPath(context.Request.Path.Value ?? ""))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        StatusCode = context.Response.StatusCode,
                        IsSuccess = false,
                        Message = "Account is not approved"
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }
            }

            await _next(context);
        }

        private static bool IsPublicPath(string path)
        {
            var publicPaths = new List<string>
            {
                "/api/v1/auth/",
                "/api/v1/museums/",
                "/api/v1/museums",
                "/swagger",
                "/swagger/"
            };
            return publicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        }
    }
}
