using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Dtos.AreaDtos
{
    public class AreaResponse
    {
        public string? Id { get; set; } 

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public AreaStatus Status { get; set; } = AreaStatus.Active;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
