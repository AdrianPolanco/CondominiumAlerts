using System.Reflection;
using CondominiumAlerts.CrossCutting.Behaviors;
using CondominiumAlerts.Features.Commands;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CondominiumAlerts.Features.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        
        services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserValidator>();
        return services;
    }
}