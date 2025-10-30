using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IDashboardSuperService
    {
     
        Task<object> GetAccountStatsAsync();
        Task<object> GetMuseumStatsAsync();
        Task<object> GetArtifactStatsAsync();
    }
}
