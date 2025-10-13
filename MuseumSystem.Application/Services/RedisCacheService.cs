using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MuseumSystem.Application.Interfaces;
using StackExchange.Redis;

public class RedisCacheService : IRedisCacheService, IDisposable
{
    private readonly IConnectionMultiplexer _connection;
    private readonly IDatabase _database;
    private readonly TimeSpan _defaultExpiryMuseum;

    public RedisCacheService(IConfiguration configuration)
    {
        var connectionString = 
            configuration.GetSection("Redis:RedisConnection").Value;

        _defaultExpiryMuseum = TimeSpan.FromHours(configuration.GetValue<double?>("Redis:ExpireTimeMuseum") ?? 3);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Redis connection string is not configured.");
        }

        try
        {
            _connection = ConnectionMultiplexer.Connect(connectionString);
            _database = _connection.GetDatabase();
        }
        catch (RedisConnectionException ex)
        {
            throw new InvalidOperationException("Failed to connect to Redis", ex);
        }
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, _defaultExpiryMuseum);
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

    public void Dispose()
    {
        _connection?.Dispose();
    }

    // Setting custom museumId with custom expiry time
    public async Task SetMuseumIdAsync(string userId, string museumId)
    {
        var key = $"user:{userId}:museumId";
        await _database.StringSetAsync(key, museumId, _defaultExpiryMuseum);
    }

}