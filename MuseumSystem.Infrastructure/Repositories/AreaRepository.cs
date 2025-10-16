using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Interface;
using MuseumSystem.Infrastructure.DatabaseSetting;
using MuseumSystem.Infrastructure.Implementation;


namespace MuseumSystem.Infrastructure.Repositories
{
    public class AreaRepository : GenericRepository<Area>, IAreaRepository
    {
        public AreaRepository(AppDbContext context) : base(context)
        {
        }
    }
}
