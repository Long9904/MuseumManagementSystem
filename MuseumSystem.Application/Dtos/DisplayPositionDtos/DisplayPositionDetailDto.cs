using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Dtos.DisplayPositionDtos
{
    public class DisplayPositionDetailDto
    {
        public string? Id { get; set; }

        public string? PositionCode { get; set; }

        public string? Description { get; set; }

        public DisplayPositionStatusEnum Status { get; set; }
   
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public string? ArtifactId { get; set; }

        public string? ArtifactName { get; set; }
    }
}
