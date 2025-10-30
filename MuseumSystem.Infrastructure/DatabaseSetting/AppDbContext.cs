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

        public DbSet<Visitor> Visitors { get; set; }

        public DbSet<Interaction> Interactions { get; set; }

        public DbSet<Exhibition> Exhibitions { get; set; }

        public DbSet<HistoricalContext> HistoricalContexts { get; set; }

        public DbSet<ArtifactHistoricalContext> ArtifactHistoricalContexts { get; set; }

        public DbSet<ExhibitionHistoricalContext> ExhibitionHistoricalContexts { get; set; }




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
            modelBuilder.Entity<Visitor>();
            modelBuilder.Entity<Interaction>();
            modelBuilder.Entity<Exhibition>();
            modelBuilder.Entity<HistoricalContext>();
            modelBuilder.Entity<ArtifactHistoricalContext>();
            modelBuilder.Entity<ExhibitionHistoricalContext>();


            // Auto apply configurations from the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<Visitor>()
        .Property(v => v.Status)
        .HasConversion<string>(); // ✅ Lưu Enum thành chuỗi ("Active", "Inactive")

            modelBuilder.Entity<ArtifactHistoricalContext>()
                .HasKey(ah => new { ah.ArtifactId, ah.HistoricalContextId });

            modelBuilder.Entity<ArtifactHistoricalContext>()
                .HasOne(ah => ah.Artifact)
                .WithMany(a => a.ArtifactHistoricalContexts)
                .HasForeignKey(ah => ah.ArtifactId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArtifactHistoricalContext>()
                .HasOne(ah => ah.HistoricalContext)
                .WithMany(h => h.ArtifactHistoricalContexts)
                .HasForeignKey(ah => ah.HistoricalContextId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========================
            // Configure many-to-many: Exhibition ↔ HistoricalContext
            // ==========================
            modelBuilder.Entity<ExhibitionHistoricalContext>()
                .HasKey(eh => new { eh.ExhibitionId, eh.HistoricalContextId });

            modelBuilder.Entity<ExhibitionHistoricalContext>()
                .HasOne(eh => eh.Exhibition)
                .WithMany(e => e.ExhibitionHistoricalContexts)
                .HasForeignKey(eh => eh.ExhibitionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExhibitionHistoricalContext>()
                .HasOne(eh => eh.HistoricalContext)
                .WithMany(h => h.ExhibitionHistoricalContexts)
                .HasForeignKey(eh => eh.HistoricalContextId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    // must here ..\MuseumManagement\MuseumManagementSystem> dotnet ef ...
    //dotnet ef migrations add AddAccountTable -p MuseumSystem.Infrastructure -s MuseumSystem.Api
    //dotnet ef database update -p MuseumSystem.Infrastructure -s MuseumSystem.Api
}
