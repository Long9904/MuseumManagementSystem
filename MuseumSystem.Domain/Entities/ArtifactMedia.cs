using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Domain.Entities
{
    public class ArtifactMedia
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public ArtifactMediaType MediaType { get; set; } = ArtifactMediaType.Image;
        public string FilePath { get; set; } = null!; // URL or relative path
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public string? MimeType { get; set; } //"image/png", "model/gltf+json"
        public ArtifactMediaStatus Status { get; set; } = ArtifactMediaStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key to Artifact
        public string ArtifactId { get; set; } = null!; 
        public Artifact Artifact { get; set; } = null!;

        // Navigation properties for different media types
        public Image? Image { get; set; }
        public Model3D? Model3D { get; set; }
    }
}
