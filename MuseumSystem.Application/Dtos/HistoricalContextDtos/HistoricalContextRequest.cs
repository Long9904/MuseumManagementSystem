using System.Collections.Generic;

namespace MuseumSystem.Application.Dtos.HistoricalContextsDtos
{
    public class HistoricalContextRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Period { get; set; }
        public string? Description { get; set; }
        // Liên kết nhiều - nhiều
        public List<string>? ArtifactIds { get; set; } = new();
        //public List<string>? ExhibitionIds { get; set; } = new();
    }
}
