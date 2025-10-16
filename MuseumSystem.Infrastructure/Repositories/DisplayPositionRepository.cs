using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Interface;
using MuseumSystem.Infrastructure.DatabaseSetting;
using MuseumSystem.Infrastructure.Implementation;

namespace MuseumSystem.Infrastructure.Repositories
{
    public class DisplayPositionRepository : GenericRepository<DisplayPosition>, IDisplayPositionRepository
    {
        public DisplayPositionRepository(AppDbContext appDb) : base(appDb)
        {
        }
    }
}
