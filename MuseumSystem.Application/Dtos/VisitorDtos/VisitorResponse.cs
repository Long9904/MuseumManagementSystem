using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Dtos.VisitorDtos
{
    public class VisitorResponse
    {
        public string? Id { get; set; }

        public required string Username { get; set; }

        public VisitorStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
