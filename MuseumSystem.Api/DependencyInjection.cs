namespace MuseumSystem.Api
{
    public static class DependencyInjection
    {
        private static void ConfigInfrastructure(IServiceCollection services, IConfiguration configuration)
        {
            Infrastructure.DependencyInjection.AddInfrastructure(services,configuration);
        }
        private static void ConfigApplication(IServiceCollection services)
        {
            Application.DependencyInjection.AddApplication(services);
        }
    }
}
