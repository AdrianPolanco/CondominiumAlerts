using Microsoft.Extensions.DependencyInjection;

namespace CondominiumAlerts.CrossCutting.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCrossCuttingConcerns(this IServiceCollection services)
    {
        return services;
    }
}