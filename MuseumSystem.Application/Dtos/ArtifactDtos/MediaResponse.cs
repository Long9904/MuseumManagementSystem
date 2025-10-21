using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Dtos.ArtifactDtos
{
    public class MediaResponse
    {
        public string? Id { get; set; }
        public ArtifactMediaType MediaType { get; set; }
        public string FilePath { get; set; } = null!; // URL or relative path
        public string? FileName { get; set; }
        public string? MimeType { get; set; } //"image/png", "model/gltf+json"
        public string? FileFormat { get; set; } // "png", "jpg", "gltf", "glb"
        public string? Caption { get; set; }
        public ArtifactMediaStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
