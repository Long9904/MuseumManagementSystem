using System.Linq;
using System.Collections.Generic;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Dtos.ExhibitionDtos
{
    public class ExhibitionResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }
        public ExhibitionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<object> HistoricalContexts { get; set; } = new();

        public ExhibitionResponse(Exhibition ex)
        {
            Id = ex.Id;
            Name = ex.Name;
            Description = ex.Description;
            Priority = ex.Priority;
            Status = ex.Status;
            StartDate = ex.StartDate;
            EndDate = ex.EndDate;

            if (ex.HistoricalContexts != null)
            {
                HistoricalContexts = ex.HistoricalContexts.Select(h => new
                {
                    historicalContextId = h.HistoricalContextId,
                    title = h.Title
                }).ToList<object>();
            }
        }
    }
}
