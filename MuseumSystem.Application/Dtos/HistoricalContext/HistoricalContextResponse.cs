using System;
using System.Collections.Generic;
using System.Linq;
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

        // Danh sách Artifact và Exhibition liên quan
        public List<string>? ArtifactIds { get; set; }
        public List<string>? ExhibitionIds { get; set; }

        public HistoricalContextResponse(HistoricalContext entity)
        {
            HistoricalContextId = entity.HistoricalContextId;
            Title = entity.Title;
            Period = entity.Period;
            Description = entity.Description;
            Status = entity.Status;

            // Mapping quan hệ
            ArtifactIds = entity.ArtifactHistoricalContexts?
                .Select(a => a.ArtifactId)
                .ToList();

            ExhibitionIds = entity.ExhibitionHistoricalContexts?
                .Select(e => e.ExhibitionId)
                .ToList();
        }
    }
}
