namespace MuseumSystem.Application.Dtos.ArtifactDtos
{
    public class ArtifactRequest
    {
        public required string ArtifactCode { get; set; }

        public required string Name { get; set; }

        public required string PeriodTime { get; set; }

        public required string Description { get; set; }

        public bool IsOriginal { get; set; }

        public double? Weight { get; set; } // kg

        public double? Height { get; set; } // cm

        public double? Width { get; set; } // cm

        public double? Length { get; set; } // cm
    }
}
