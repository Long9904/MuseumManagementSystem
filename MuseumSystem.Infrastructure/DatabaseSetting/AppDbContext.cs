using Microsoft.EntityFrameworkCore;
using MuseumSystem.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MuseumSystem.Infrastructure.DatabaseSetting
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // Example DbSet, replace with actual entities
        public DbSet<Account> Accounts { get; set; }
        // Configure DbSets
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example entity configuration
            modelBuilder.Entity<Account>();


            // Auto apply configurations from the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
    // must here ..\MuseumManagement\MuseumManagementSystem> dotnet ef ...
    //dotnet ef migrations add AddAccountTable -p MuseumSystem.Infrastructure -s MuseumSystem.Api
    //dotnet ef database update -p MuseumSystem.Infrastructure -s MuseumSystem.Api
}
