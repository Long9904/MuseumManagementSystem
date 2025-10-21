using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Application.Services;
using MuseumSystem.Application.Utils;

namespace MuseumSystem.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
          
            // services.AddScoped<IYourService, YourServiceImplementation>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IGenerateTokenService, GenerateTokenService>();
            services.AddScoped<IMuseumService, MuseumService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IDisplayPositionService, DisplayPositionService>();
            services.AddScoped<IArtifactService, ArtifactService>();
            services.AddScoped<ISeedService, SeedService>();
            services.AddScoped<IArtifactMediaService, ArtifactMediaService>();

            // Redis caching service
            services.AddSingleton<IRedisCacheService, RedisCacheService>();

            // utils
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        }
    }
}
