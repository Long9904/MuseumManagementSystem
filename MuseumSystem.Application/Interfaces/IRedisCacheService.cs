namespace MuseumSystem.Application.Interfaces
{
    public interface IRedisCacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan expiryTime);
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);

        Task SetMuseumIdAsync(string userId, string museumId);

        Task RemoveMuseumIdAsync(string userId);
    }
}
