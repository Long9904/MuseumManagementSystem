namespace MuseumSystem.Domain.Entities
{
    public class Model3D
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Caption { get; set; }
        public string? FileFormat { get; set; }
        public int? PolygonCount { get; set; }
        public int? TextureCount { get; set; }
        public string? BoundingBox { get; set; }

        // ArtifactMedia relationship
        public required string ArtifactMediaId { get; set; }
        public required ArtifactMedia ArtifactMedia { get; set; }
    }
}
