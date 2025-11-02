using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Dtos.ExhibitionDtos
{
    public class ExhibitionRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, 10)]
        public int Priority { get; set; } = 0;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // ✅ Danh sách các HistoricalContext được gán vào Exhibition
        public List<string>? HistoricalContextIds { get; set; } = new();
    }
}
