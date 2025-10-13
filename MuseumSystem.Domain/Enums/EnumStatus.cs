using Microsoft.AspNetCore.Cors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Domain.Enums
{
    public enum EnumStatus  
    {
        [Display(Name = "Active")]
        Active,
        [Display(Name = "Inactive")]
        Inactive
    }
    public enum EnumOrderBy
    {
        [Display(Name = "Ascending")]
        Asc,
        [Display(Name = "Descending")]
        Desc
    }
}
