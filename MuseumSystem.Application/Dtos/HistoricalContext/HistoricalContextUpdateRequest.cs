﻿namespace MuseumSystem.Application.Dtos.HistoricalContexts
{
    public class HistoricalContextUpdateRequest
    {
        public string? Title { get; set; }
        public string? Period { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
    }
}
