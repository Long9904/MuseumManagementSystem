
using MuseumSystem.Application.Dtos.HistoricalContextsDtos;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Dtos.ExhibitionDtos
{
    public class ExhibitionResponseV2
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }
        public ExhibitionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<HistoricalContextResponseV2>? HistoricalContexts { get; set; } = new();
    }
}
