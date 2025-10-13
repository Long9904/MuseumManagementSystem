namespace MuseumSystem.Application.Interfaces
{
    public interface ICurrentUser
    {
        string ?UserId { get; }
        string ?MuseumId { get; }
    }
}
