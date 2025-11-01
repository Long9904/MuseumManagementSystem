using MuseumSystem.Domain.Enums;

namespace MuseumSystem.Application.Dtos.MuseumDtos
{
    public class MuseumResponseV1
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public EnumStatus Status { get; set; }
    }
}
