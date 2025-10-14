using System.Security.Claims;
using MuseumSystem.Application.Interfaces;

namespace MuseumSystem.Api.Middleware
{
    public class MuseumContextMiddleware
    {
        private readonly RequestDelegate _next;

        public MuseumContextMiddleware(RequestDelegate next)
        {
            _next = next;      
        }

        public async Task InvokeAsync(HttpContext context, IRedisCacheService cacheService)
        {
            var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var cacheKey = $"user:{userId}:museumId";
                var museumId = await cacheService.GetAsync<string>(cacheKey);

                if (!string.IsNullOrEmpty(museumId))
                    context.Items["MuseumId"] = museumId;
            }

            await _next(context);
        }
    }
}
