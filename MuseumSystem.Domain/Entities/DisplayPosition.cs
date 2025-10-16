using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Domain.Entities
{
    public class DisplayPosition
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string DisplayPositionName { get; set; } = string.Empty;

        public string PositionCode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DisplayPositionStatusEnum Status { get; set; } = DisplayPositionStatusEnum.Active;

        // Foreign key to Area
        public required string AreaId { get; set; }

        public required Area Area { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // relationships to artifact 1-1
        public Artifact? Artifact { get; set; }
        public string? ArtifactId { get; set;  }


    }
}
