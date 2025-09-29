using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Infrastructure.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Infrastructure
{
    public static class DependencyInjection
    {
       public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
           
        }
    }
}
