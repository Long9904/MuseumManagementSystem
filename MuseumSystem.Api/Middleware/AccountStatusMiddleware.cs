using System.Security.Claims;
using System.Text.Json;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Enums;

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

            if (context.User.IsInRole("SuperAdmin") || context.User.IsInRole("Visitor"))
            {
                await _next(context);
                return;
            }

            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                                ?? throw new UnauthorizedAccessException("User is not login");
                var account = await unitOfWork.AccountRepository.GetByIdAsync(userId) 
                              ?? throw new UnauthorizedAccessException("Account not found");
                if (account.MuseumId == null)
                {
                    throw new UnauthorizedAccessException("User is not associated with any museum.");
                }
                var museum = await unitOfWork.MuseumRepository.GetByIdAsync(account.MuseumId)
                                ?? throw new UnauthorizedAccessException("Museum not found for the account.");
                var status = museum.Status;


                var path = context.Request.Path.Value?.ToLower();
                if ((status == EnumStatus.Pending || status == EnumStatus.Rejected ) && !IsPublicPath(context.Request.Path.Value ?? ""))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        StatusCode = context.Response.StatusCode,
                        IsSuccess = false,
                        Message = "Museum is not aproved or has been rejected"
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
                "/swagger",
                "/swagger/"
            };
            return publicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        }
    }
}
