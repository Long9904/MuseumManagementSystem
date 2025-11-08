using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Dtos.HistoricalContextsDtos
{
    public class HistoricalArtifactAssignRequest
    {
        public List<string> ArtifactIds { get; set; } = new();
    }
}
