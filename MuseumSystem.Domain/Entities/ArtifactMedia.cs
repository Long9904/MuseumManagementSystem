using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Domain.Entities
{
    public class ArtifactMedia
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public ArtifactMediaType MediaType { get; set; } = ArtifactMediaType.Image;
        public string FilePath { get; set; } = null!; // URL or relative path
        public string? FileName { get; set; }
        public string? MimeType { get; set; } //"image/png", "model/gltf+json"
        public ArtifactMediaStatus Status { get; set; } = ArtifactMediaStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign key to Artifact
        public required string ArtifactId { get; set; }
        public required Artifact Artifact { get; set; }

        // Navigation properties for different media types
        public Image? Image { get; set; }
        public Model3D? Model3D { get; set; }
    }
}
