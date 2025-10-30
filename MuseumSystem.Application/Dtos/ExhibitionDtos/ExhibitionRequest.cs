using MuseumSystem.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MuseumSystem.Application.Dtos.ExhibitionDtos
{
    public class ExhibitionRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, 10)]
        public int Priority { get; set; } = 0;

        public ExhibitionStatus Status { get; set; } = ExhibitionStatus.Daily;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        [Required]
        public string MuseumId { get; set; } = string.Empty;
    }
}
