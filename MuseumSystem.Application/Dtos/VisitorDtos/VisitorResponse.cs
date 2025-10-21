using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Dtos.VisitorDtos
{
    public class VisitorResponse
    {
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
