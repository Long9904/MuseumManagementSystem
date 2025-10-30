using MuseumSystem.Domain.Enums;
using System;

namespace MuseumSystem.Application.Dtos.ExhibitionDtos
{
    public class ExhibitionUpdateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Priority { get; set; }
        public ExhibitionStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
