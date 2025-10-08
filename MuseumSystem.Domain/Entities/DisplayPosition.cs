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

        public Area Area { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign key to ArtifactDisplay (many-to-many relationship)
        public ICollection<ArtifactDisplay> ArtifactDisplays { get; set; } = new List<ArtifactDisplay>();

    }
}
