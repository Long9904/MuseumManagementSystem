using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Dtos.ExhibitionDtos
{
    public class ExhibitionHistoricalAssignRequest
    {
        public List<string> HistoricalContextIds { get; set; } = new();
    }
}

