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
        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Museum> Museums { get; set; }

        public DbSet<Area> Areas { get; set; }

        public DbSet<DisplayPosition> DisplayPositions { get; set; }

        public DbSet<Artifact> Artifacts { get; set; }

        public DbSet<ArtifactMedia> ArtifactMedias { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Model3D> Model3Ds { get; set; }
        // Configure DbSets
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example entity configuration
            modelBuilder.Entity<Role>();
            modelBuilder.Entity<Account>();
            modelBuilder.Entity<Museum>();
            modelBuilder.Entity<Area>();
            modelBuilder.Entity<DisplayPosition>();
            modelBuilder.Entity<Artifact>();
            modelBuilder.Entity<ArtifactMedia>();
            modelBuilder.Entity<Image>();
            modelBuilder.Entity<Model3D>();

            // Auto apply configurations from the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
    // must here ..\MuseumManagement\MuseumManagementSystem> dotnet ef ...
    //dotnet ef migrations add AddAccountTable -p MuseumSystem.Infrastructure -s MuseumSystem.Api
    //dotnet ef database update -p MuseumSystem.Infrastructure -s MuseumSystem.Api
}
