using System.Security.Claims;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Application.Utils;

namespace MuseumSystem.Api.Middleware
{
    public class MuseumContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly GetCurrentUserLogin _getCurrentUserLogin;
        private readonly IRedisCacheService _cacheService;
        private readonly ICurrentUserLogin _currentUserLogin;

        public MuseumContextMiddleware(
            RequestDelegate next, 
            GetCurrentUserLogin getCurrentUserLogin, 
            IRedisCacheService cacheService, 
            ICurrentUserLogin currentUserLogin)
        {
            _next = next;
            _getCurrentUserLogin = getCurrentUserLogin;
            _cacheService = cacheService;
            _currentUserLogin = currentUserLogin;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = _getCurrentUserLogin.UserId;
            if (!string.IsNullOrEmpty(userId))
            {
                var cacheKey = $"user:{userId}:museumId";
                var museumId = await _cacheService.GetAsync<string>(cacheKey);

                if (!string.IsNullOrEmpty(museumId))
                {
                    context.Items["MuseumId"] = museumId;
                }
            }

            await _next(context);
        }
    }
}
