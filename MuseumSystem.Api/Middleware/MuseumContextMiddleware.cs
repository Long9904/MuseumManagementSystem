using System.Security.Claims;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Interface;

namespace MuseumSystem.Api.Middleware
{
    public class MuseumContextMiddleware
    {
        private readonly RequestDelegate _next;

        public MuseumContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context, 
            IRedisCacheService cacheService, 
            IAccountRepository _accountRepository, 
            ILogger<MuseumContextMiddleware> _logger)
        {
            var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var cacheKey = $"user:{userId}:museumId";
                var museumId = await cacheService.GetAsync<string>(cacheKey);

                // If cache was timeout or not exist, get from db and set to cache redis for next time
                if (string.IsNullOrEmpty(museumId))
                {
                    _logger.LogInformation($"Cache miss for userId: {userId}. Fetching museumId from database.");
                    var account = await _accountRepository.GetByIdAsync(userId);
                    museumId = account?.MuseumId;
                    if (!string.IsNullOrEmpty(museumId))
                    {
                        await cacheService.SetAsync(cacheKey, museumId, TimeSpan.FromMinutes(30));
                    }
                }

                if (!string.IsNullOrEmpty(museumId))
                    context.Items["MuseumId"] = museumId;
            }

            await _next(context);
        }
    }
}
