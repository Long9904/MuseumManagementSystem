namespace MuseumSystem.Domain.Entities
{
    public class Image
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsThumbnail { get; set; } = false;
        public string? Caption { get; set; }

        // ArtifactMedia relationship
        public required string ArtifactMediaId { get; set; }
        public required ArtifactMedia ArtifactMedia { get; set; }
    }
}
