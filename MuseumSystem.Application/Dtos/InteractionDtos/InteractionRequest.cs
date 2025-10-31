using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Dtos.InteractionDtos
{
    public class InteractionRequest
    {
        public required string ArtifactId { get; set; }
        public string? Comment { get; set; }
        public double? Rating { get; set; }
    }
}
