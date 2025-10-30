using System.Collections.Generic;

namespace MuseumSystem.Application.Dtos.HistoricalContexts
{
    public class HistoricalContextRequest
    {
        public string Title { get; set; }
        public string? Period { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        // Liên kết nhiều - nhiều
        public List<string> ArtifactIds { get; set; } = new();
        public List<string> ExhibitionIds { get; set; } = new();
    }
}
