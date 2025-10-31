
namespace MuseumSystem.Application.Dtos.InteractionDtos
{
    public class VisitorInteractionResponse
    {
        public string? InteractionId { get; set; }
        public string? Comment { get; set; }

        public double? Rating { get; set; }

        public DateTime CreatedAt { get; set; }

        // Artifact details
        public string? ArtifactId { get; set; }
        public string? ArtifactName { get; set; }

        // Museum details
        public string? MuseumId { get; set; }
        public string? MuseumName { get; set; }
    }
}
