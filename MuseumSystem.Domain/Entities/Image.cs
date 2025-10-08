namespace MuseumSystem.Domain.Entities
{
    public class Image
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsThumbnail { get; set; } = false;
        public string? Caption { get; set; }

        // ArtifactMedia relationship
        public string ArtifactMediaId { get; set; } = null!;
        public ArtifactMedia ArtifactMedia { get; set; } = null!;
    }
}
