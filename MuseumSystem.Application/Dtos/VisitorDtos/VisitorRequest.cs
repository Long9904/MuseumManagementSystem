using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Dtos.VisitorDtos
{
    public class VisitorRequest
    {
        [Required]
        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; }
    }
}
