using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Interface;
using MuseumSystem.Infrastructure.DatabaseSetting;
using MuseumSystem.Infrastructure.Implementation;

namespace MuseumSystem.Infrastructure.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext appDb) : base(appDb)
        {
        }
    }
}
