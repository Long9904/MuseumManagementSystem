namespace MuseumSystem.Application.Dtos.AIChatDtos
{
    public class ChatContext
    {
        public string? LastArtifactId { get; set; }
        public string? LastArtifactName { get; set; }

        public string? LastAreaId { get; set; }
        public string? LastAreaName { get; set; }

        public string? LastExhibitionId { get; set; }
        public string? LastExhibitionName { get; set; }

        public string? LastAction { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
