using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Dtos.ArtifactDtos
{
    public class ArtifactResponse
    {
        public string? Id { get; set; }

        public string? ArtifactCode { get; set; }

        public string? Name { get; set; }

        public string? PeriodTime { get; set; }

        public string? Description { get; set; }

        public bool IsOriginal { get; set; }

        public double? Weight { get; set; } // kg

        public double? Height { get; set; } // cm

        public double? Width { get; set; } // cm

        public double? Length { get; set; } // cm

        public ArtifactStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? DisplayPositionId { get; set; }

        public string? DisplayPositionName { get; set; }

        public string? AreaId { get; set; }

        public string? AreaName { get; set; }
    }
}
