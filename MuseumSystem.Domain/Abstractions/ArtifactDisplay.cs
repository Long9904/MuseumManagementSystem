using MuseumSystem.Domain.Entities;

namespace MuseumSystem.Domain.Abstractions
{
    public class ArtifactDisplay
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ArtifactId { get; set; } = null!;
        public Artifact Artifact { get; set; } = null!;

        public string DisplayPositionId { get; set; } = null!;
        public DisplayPosition DisplayPosition { get; set; } = null!;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
    }
}
