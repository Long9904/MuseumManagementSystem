using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Dtos.AreaDtos
{
    public class AreaResponse
    {
        public string? Id { get; set; } 

        public string? Name { get; set; }

        public string? Description { get; set; } 

        public AreaStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
