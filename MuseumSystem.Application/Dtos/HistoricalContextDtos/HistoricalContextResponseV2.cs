
namespace MuseumSystem.Application.Dtos.HistoricalContextsDtos
{
    public class HistoricalContextResponseV2
    {
        public string? Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Period { get; set; }
        public string? Description { get; set; }

        public List<ListArtifactInHistoricalContext>? Artifacts { get; set; } = new();

    }
    public class ListArtifactInHistoricalContext
    {
        public string ArtifactId { get; set; } = string.Empty;
        public string ArtifactName { get; set; } = string.Empty;
    }

}
