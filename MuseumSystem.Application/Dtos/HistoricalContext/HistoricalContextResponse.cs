using MuseumSystem.Application.Dtos.ArtifactDtos;
using MuseumSystem.Domain.Entities;

namespace MuseumSystem.Application.Dtos.HistoricalContexts
{
    public class HistoricalContextResponse
    {
        public string HistoricalContextId { get; set; }
        public string Title { get; set; }
        public string? Period { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public List<ArtifactResponse> Artifacts { get; set; }

        public HistoricalContextResponse(HistoricalContext entity)
        {
            HistoricalContextId = entity.HistoricalContextId;
            Title = entity.Title;
            Period = entity.Period;
            Description = entity.Description;
            Status = entity.Status.ToString();

            Artifacts = entity.ArtifactHistoricalContexts?
                .Where(ah => ah.Artifact != null)
                .Select(ah => new ArtifactResponse
                {
                    Id = ah.Artifact.Id,
                    Name = ah.Artifact.Name
                })
                .ToList() ?? new List<ArtifactResponse>();
        }
    }
}
