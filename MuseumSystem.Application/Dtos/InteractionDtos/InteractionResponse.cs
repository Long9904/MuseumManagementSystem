using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Dtos.InteractionDtos
{
    public class InteractionResponse
    {
        public string Id { get; set; }
        public string VisitorId { get; set; }
        public string ArtifactId { get; set; }
        public string? Comment { get; set; }
        public double? Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? VisitorPhoneNumber { get; set; }
        public string? ArtifactName { get; set; }
        public string? ArtifactCode { get; set; }
    }
}
