using CondominiumAlerts.Infrastructure.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CondominiumAlerts.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddNpgsql<ApplicationDbContext>(configuration.GetConnectionString("Database")!);
        return services;
    }
}