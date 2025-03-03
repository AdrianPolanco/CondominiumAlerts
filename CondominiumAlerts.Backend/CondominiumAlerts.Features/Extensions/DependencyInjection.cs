using System.Reflection;
using CondominiumAlerts.CrossCutting.Behaviors;
using CondominiumAlerts.Features.Features.Condominium.Add;
using CondominiumAlerts.Features.Features.Condominium.Get;
using CondominiumAlerts.Features.Features.Condominium.Join;
using CondominiumAlerts.Features.Features.Posts.Get;
using CondominiumAlerts.Features.Features.Users.Register;
using FluentValidation;
using LightResults;
using MediatR;
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
        services.AddScoped<IValidator<AddCondominiumCommand>, AddCondominiumValidator>();
        services.AddScoped<IValidator<GetCondominiumCommand>, GetCondominiumValidator>();

        services.AddScoped<IRequestHandler<GetPostsCommand, Result<List<GetPostsResponse>>>, GetPostsHandler>();
        services.AddTransient<EmailConfirmationJob>();
        return services;
    }
}