namespace MuseumSystem.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? MuseumId { get; }
    }
}
