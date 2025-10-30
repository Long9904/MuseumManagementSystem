using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Application.Services;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Interface;
using MuseumSystem.Infrastructure.Implementation;
using MuseumSystem.Infrastructure.Repositories;


namespace MuseumSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<IMuseumRepository, MuseumRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IDisplayPositionRepository, DisplayPositionRepository>();
            services.AddScoped<IArtifactRepository, ArtifactRepository>();
            services.AddScoped<IInteractionService, InteractionService>();
            services.AddScoped<IVisitorService, VisitorService>();
            services.AddScoped<IExhibitionService, ExhibitionService>();
            services.AddScoped<IHistoricalContextService, HistoricalContextService>();
        }
    }
}
