using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Dtos.MuseumDtos
{
    public class MuseumRequest
    {      
        public string Name { get; set; } = string.Empty;       
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
