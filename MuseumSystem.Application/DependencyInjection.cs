using Microsoft.Extensions.DependencyInjection;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
          
            // services.AddScoped<IYourService, YourServiceImplementation>();
            services.AddScoped<IRoleService, RoleService>();
        }
    }
}
