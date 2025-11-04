using MuseumSystem.Application.Dtos.AIChatDtos;
using MuseumSystem.Application.Interfaces;

namespace MuseumSystem.Application.Services
{
    internal class ChatContextService : IChatContextService
    {
        private readonly IRedisCacheService _cache;
        private readonly TimeSpan _contextExpiry = TimeSpan.FromMinutes(30);

        public ChatContextService(IRedisCacheService cache)
        {
            _cache = cache;
        }

        private string GetContextKey(string userId) => $"chat:user:{userId}:context";

        public async Task<ChatContext> GetContextAsync(string userId)
        {
            var key = GetContextKey(userId);
            return await _cache.GetAsync<ChatContext>(key) ?? new ChatContext();
        }

        public async Task SetContextAsync(string userId, ChatContext context)
        {
            var key = GetContextKey(userId);
            await _cache.SetAsync(key, context, _contextExpiry);
        }

        public async Task ClearContextAsync(string userId)
        {
            var key = GetContextKey(userId);
            await _cache.RemoveAsync(key);
        }
    }
}
