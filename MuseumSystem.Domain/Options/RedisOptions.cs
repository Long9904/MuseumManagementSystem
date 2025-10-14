namespace MuseumSystem.Domain.Options
{
    public class RedisOptions
    {
        public string RedisConnection { get; set; } = string.Empty;
        public double ExpireTimeMuseum { get; set; } = 3; // default 3 hours
    }
}
