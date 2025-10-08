using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Domain.Entities
{
    public class Artifact
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ArtifactCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string PeriodTime { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsOriginal { get; set; } = true;

        public double? Weight { get; set; } // kg

        public double? Height { get; set; } // cm

        public double? Width { get; set; } // cm

        public double? Length { get; set; } // cm

        public ArtifactStatus Status { get; set; } = ArtifactStatus.OnDisplay;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign key to media
        public ICollection<ArtifactMedia> ArtifactMedias { get; set; } = new List<ArtifactMedia>();

        // Foreign key to ArtifactDisplay (many-to-many relationship)
        public ICollection<ArtifactDisplay> DisplayHistory { get; set; } = new List<ArtifactDisplay>();

    }
}
