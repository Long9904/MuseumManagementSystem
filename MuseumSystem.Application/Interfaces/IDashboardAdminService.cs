using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IDashboardAdminService
    {
        Task<object> GetArtifactStatsAsync();
        Task<object> GetStaffStatsAsync();
        
    }
}
