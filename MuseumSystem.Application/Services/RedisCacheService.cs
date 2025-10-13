using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MuseumSystem.Application.Interfaces;
using StackExchange.Redis;

namespace MuseumSystem.Application.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;

        public RedisCacheService(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("Redis:RedisConnection").Value;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("Redis connection string is not configured.");
            }
            var connection = ConnectionMultiplexer.Connect(connectionString);
            _database = connection.GetDatabase();
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _database.StringGetAsync(key);
            if (json.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(json!);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }
    }
}
