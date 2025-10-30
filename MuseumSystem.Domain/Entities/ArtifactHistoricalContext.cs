namespace MuseumSystem.Domain.Entities
{
    public class ArtifactHistoricalContext
    {
        public string ArtifactId { get; set; }
        public Artifact Artifact { get; set; }

        public string HistoricalContextId { get; set; }
        public HistoricalContext HistoricalContext { get; set; }
    }
}
