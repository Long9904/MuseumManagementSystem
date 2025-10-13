using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Dtos.MuseumDtos
{
    public class MuseumFilterDtos
    {
        public string Name { get; set; } 
        public string Location { get; set; } 
        public string Description { get; set; } 
        public EnumStatus Status { get; set; } 
        public DateTime CreateAt { get; set; } 
        public DateTime? UpdateAt { get; set; }
        public EnumOrderBy Orderby { get; set; }
    }
}
