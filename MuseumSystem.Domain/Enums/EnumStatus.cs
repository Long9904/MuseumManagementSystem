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
    public enum ExhibitionStatus
    {
        [Display(Name = "Upcoming")]
        Upcoming = 0,

        [Display(Name = "Active")]
        Active = 1,

        [Display(Name = "Expired")]
        Expired = 2,

        [Display(Name = "Daily")]
        Daily = 3,

        [Display(Name = "Deleted")]
        Deleted = 4
    }
    public enum HistoricalStatus
    {
        [Display(Name = "Active")]
        Active = 0,

        [Display(Name = "Deleted")]
        Deleted = 1
    }

    public enum EnumOrderBy
    {
        [Display(Name = "Ascending")]
        Asc,
        [Display(Name = "Descending")]
        Desc
    }
}
