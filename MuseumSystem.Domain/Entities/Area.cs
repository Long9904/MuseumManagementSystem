using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Domain.Entities
{
    public class Area
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public AreaStatus Status { get; set; } = AreaStatus.Active;

        public string MuseumId { get; set; } = string.Empty;

        public Museum Museum { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Display positions in this area
        public ICollection<DisplayPosition> DisplayPositions { get; set; } = new List<DisplayPosition>();
    }
}
