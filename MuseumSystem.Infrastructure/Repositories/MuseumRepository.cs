using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Interface;
using MuseumSystem.Infrastructure.DatabaseSetting;
using MuseumSystem.Infrastructure.Implementation;

namespace MuseumSystem.Infrastructure.Repositories
{
    public class MuseumRepository : GenericRepository<Museum>, IMuseumRepository
    {
        public MuseumRepository(AppDbContext appDb) : base(appDb)
        {
        }
     
    }
}
