using MuseumSystem.Domain.Entities;
using MuseumSystem.Infrastructure.DatabaseSetting;
using MuseumSystem.Infrastructure.Implementation;
using MuseumSystem.Infrastructure.Interface;

namespace MuseumSystem.Infrastructure.Repositories
{
    public class AreaRepository : GenericRepository<Area>, IAreaRepository
    {
        public AreaRepository(AppDbContext context) : base(context)
        {
        }
    }
}
