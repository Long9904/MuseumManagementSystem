using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuseumSystem.Domain.Entities
{

    public class ExhibitionHistoricalContext
    {


        public string ExhibitionId { get; set; }
        public Exhibition Exhibition { get; set; }

        public string HistoricalContextId { get; set; } 
        public HistoricalContext HistoricalContext { get; set; }
    }
}
