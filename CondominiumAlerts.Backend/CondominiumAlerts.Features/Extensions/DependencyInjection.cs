using System.Reflection;
using CondominiumAlerts.CrossCutting.Behaviors;
using CondominiumAlerts.Features.Features.Condominium.Join;
using CondominiumAlerts.Features.Features.Users.Register;
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
            //config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        
        services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserValidator>();
        services.AddScoped<IValidator<JoinCondominiumCommand>, JoinCondominiumValidator>();
        services.AddTransient<EmailConfirmationJob>();
        return services;
    }
}