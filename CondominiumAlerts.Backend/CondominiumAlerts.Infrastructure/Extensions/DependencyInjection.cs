using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Auth;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using CondominiumAlerts.Infrastructure.Persistence.Context;
using CondominiumAlerts.Infrastructure.Persistence.Repositories;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CondominiumAlerts.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNpgsql<ApplicationDbContext>(configuration.GetConnectionString("DefaultConnection")!);
        services.AddSingleton<IAuthenticationProvider, AuthenticationProvider>();
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        return services;
    }
}