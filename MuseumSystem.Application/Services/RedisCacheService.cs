using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Options;
using StackExchange.Redis;

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _connection;
    private readonly IDatabase _database;
    private readonly TimeSpan _defaultExpiryMuseum;

    public RedisCacheService(IOptions<RedisOptions> options)
    {
        var redisOptions = options.Value;

        if (string.IsNullOrEmpty(redisOptions.RedisConnection))
            throw new ArgumentNullException(nameof(redisOptions.RedisConnection), "Redis connection string is not configured.");

        _defaultExpiryMuseum = TimeSpan.FromHours(redisOptions.ExpireTimeMuseum);

        try
        {
            _connection = ConnectionMultiplexer.Connect(redisOptions.RedisConnection);
            _database = _connection.GetDatabase();
        }
        catch (RedisConnectionException ex)
        {
            throw new InvalidOperationException("Failed to connect to Redis", ex);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiryTime)
    {
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, expiryTime);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _database.StringGetAsync(key);
        return json.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(json!);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    // Setting custom museumId with custom expiry time
    public async Task SetMuseumIdAsync(string userId, string museumId)
    {
        var key = $"user:{userId}:museumId";
        await SetAsync(key, museumId, _defaultExpiryMuseum);
    }

}