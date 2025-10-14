using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Options;
using StackExchange.Redis;

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer? _connection;
    private readonly IDatabase? _database;
    private readonly TimeSpan _defaultExpiryMuseum;
    private readonly ILogger<RedisCacheService> _logger;

    // Fallback in-memory cache if Redis is not available
    private readonly bool _useMemoryFallback = false;
    private readonly Dictionary<string, (string value, DateTime expireAt)> _memoryCache = new();

    public RedisCacheService(IOptions<RedisOptions> options, ILogger<RedisCacheService> logger)
    {
        _logger = logger;
        var redisOptions = options.Value;
        _defaultExpiryMuseum = TimeSpan.FromHours(redisOptions.ExpireTimeMuseum);

        if (string.IsNullOrEmpty(redisOptions.RedisConnection))
        {
            _useMemoryFallback = true;
            _logger.LogWarning("Redis connection string is not provided. Falling back to in-memory cache.");
            return;
        }
       
        try
        {
            _connection = ConnectionMultiplexer.Connect(redisOptions.RedisConnection);
            _database = _connection.GetDatabase();
        }
        catch (RedisConnectionException)
        {
            _useMemoryFallback = true;
            _logger.LogError("Failed to connect to Redis. Falling back to in-memory cache.");
        }
    }


    public async Task SetAsync<T>(string key, T value, TimeSpan expiryTime)
    {
        var json = JsonSerializer.Serialize(value);

        if (_useMemoryFallback)
        {
            _memoryCache[key] = (json, DateTime.UtcNow.Add(expiryTime));
        }
        else
        {
            await _database!.StringSetAsync(key, json, expiryTime);
        }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        if (_useMemoryFallback)
        {
            if (_memoryCache.TryGetValue(key, out var entry))
            {
                if (entry.expireAt < DateTime.UtcNow)
                {
                    _memoryCache.Remove(key, out _);
                    return default;
                }
                return JsonSerializer.Deserialize<T>(entry.value);
            }
            return default;
        }
        else
        {
            var json = await _database!.StringGetAsync(key);
            return json.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(json!);
        }
    }

    public async Task RemoveAsync(string key)
    {
        if (_useMemoryFallback)
        {
            _memoryCache.Remove(key, out _);
        }
        else
        {
            await _database!.KeyDeleteAsync(key);
        }
    }

    // Setting custom museumId with custom expiry time
    public async Task SetMuseumIdAsync(string userId, string museumId)
    {
        var key = $"user:{userId}:museumId";
        await SetAsync(key, museumId, _defaultExpiryMuseum);
    }

}