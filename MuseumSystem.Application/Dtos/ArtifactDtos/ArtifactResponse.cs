using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Dtos.ArtifactDtos
{
    public class ArtifactResponse
    {
        public string? Id { get; set; }

        public string? ArtifactCode { get; set; }

        public string? Name { get; set; }

        public string? PeriodTime { get; set; }

        public bool IsOriginal { get; set; }

        public ArtifactStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? DisplayPositionId { get; set; }

        public string? DisplayPositionName { get; set; }

        public string? DisplayPositionDescription { get; set; }

        public string? AreaId { get; set; }

        public string? AreaName { get; set; }
        public string? AreaDescription { get; set; }
    }
}
