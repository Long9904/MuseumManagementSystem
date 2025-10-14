using System.Text.Json.Serialization;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Domain.Entities
{
    public class DisplayPosition
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string PositionCode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DisplayPositionStatusEnum Status { get; set; } = DisplayPositionStatusEnum.Available;

        // Foreign key to Area
        public string AreaId { get; set; } = string.Empty;

        [JsonIgnore]
        public Area Area { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // relationships to artifact 1-1
        public Artifact? Artifact { get; set; }
        public string ArtifactId { get; set;  } = string.Empty;


    }
}
