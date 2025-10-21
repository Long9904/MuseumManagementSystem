using Microsoft.AspNetCore.Http;

namespace MuseumSystem.Application.Dtos.ArtifactDtos
{
    public class MediaRequest
    {
        public IFormFile File { get; set; } = null!;

        public string? Caption { get; set; }
    }
}
