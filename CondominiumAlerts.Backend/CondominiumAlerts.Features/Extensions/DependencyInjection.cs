using System.Reflection;
using CondominiumAlerts.CrossCutting.Behaviors;
using CondominiumAlerts.Domain.Aggregates.ValueObjects;
using CondominiumAlerts.Features.Features.Condominiums.Add;
using CondominiumAlerts.Features.Features.Condominiums.Get;
using CondominiumAlerts.Features.Features.Condominiums.GetCondominiumsJoinedByUser;
using CondominiumAlerts.Features.Features.Condominiums.Join;
using CondominiumAlerts.Features.Features.Condominiums.Summaries;
using CondominiumAlerts.Features.Features.Posts.Get;
using CondominiumAlerts.Features.Features.Users.GetCondominiumsUsers;
using CondominiumAlerts.Features.Features.PriorityLevels.Add;
using CondominiumAlerts.Features.Features.PriorityLevels.Delete;
using CondominiumAlerts.Features.Features.PriorityLevels.Get;
using CondominiumAlerts.Features.Features.PriorityLevels.GetById;
using CondominiumAlerts.Features.Features.PriorityLevels.Update;
using CondominiumAlerts.Features.Features.Users.Register;
using CondominiumAlerts.Features.Features.Users.Update;
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
        services.AddScoped<IValidator<GetCondominiumsJoinedByUserCommand>, GetCondominiumsJoinedByUserValidator>();
        services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserValidator>();
        services.AddScoped<IValidator<GetPriorityLevelsQuery>, GetPriorityLevelValidator>();
        services.AddScoped<IValidator<AddPriorityLevelCommand>, AddPriorityLevelValidator>();
        services.AddScoped<IValidator<UpdatePriorityLevelCommand>, UpdatePriorityLevelValidator>();
        services.AddScoped<IValidator<DeletePriorityLevelCommand>, DeletePriorityLevelValidator>();
        services.AddScoped<IValidator<GetByIdPriorityLevelQuery>, GetByIdPriorityLevelValidator>();
        services.AddScoped<IValidator<Address>, AddressValidator>();

        services.AddScoped<IValidator<GetCondominiumsUsersCommand>, GetCondominiumsUsersValidator>();

        services.AddScoped<IRequestHandler<GetPostsCommand, Result<List<GetPostsResponse>>>, GetPostsHandler>();


        services.AddTransient<EmailConfirmationJob>();
        services.AddTransient<MessagesSummarizationJob>();

        services.AddScoped<BasicUpdateUserStrategy>();
        services.AddScoped<IUpdateUserStrategy>(sp => 
        {
            var basic = sp.GetRequiredService<BasicUpdateUserStrategy>();
            return basic;
        });
        services.Decorate<IUpdateUserStrategy, UpdateUserWithPhotoStrategy>();
        
        return services;
    }
}