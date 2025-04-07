using System.Net;
using System.Net.Mail;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Auth;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using CondominiumAlerts.Infrastructure.Persistence.Context;
using CondominiumAlerts.Infrastructure.Persistence.Repositories;
using CondominiumAlerts.Infrastructure.Services;
using CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;
using CondominiumAlerts.Infrastructure.Services.Cancellation;
using CondominiumAlerts.Infrastructure.Settings;
using Coravel;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using Polly;
using RestSharp;

namespace CondominiumAlerts.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")!), ServiceLifetime.Scoped);
        services.AddSingleton<IAuthenticationProvider, AuthenticationProvider>();
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<ICondominiumRepository, CondominiumRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        
        services.Configure<AISettings>(configuration.GetSection("AISettings"));

        /*services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<AISettings>>().Value;

            return new ChatClient(settings.Model, settings.AIKey);
        });*/
        
        services.AddHttpClient<IAiService, AiService>((sp,client) =>
        {
            var settings = sp.GetRequiredService<IOptions<AISettings>>().Value;
            client.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.AIKey}");
        });



        // Registrar política de reintentos con Polly
        services.AddSingleton<IAsyncPolicy>(policy => Policy
            .Handle<SmtpCommandException>() // Maneja excepciones de MailKit
            .Or<TimeoutException>()
            .Or<InvalidOperationException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
                (exception, timeSpan, retryCount, context) =>
                {
                    // Log o manejo del fallo de reintento
                    var logger = context["Logger"] as ILogger<EmailService>;
                    logger?.LogWarning($"Error en reintento #{retryCount}: {exception.Message}. Esperando {timeSpan.TotalSeconds} segundos.");
                })
        );
        services.AddQueue();
        services.AddSignalR();

        services.AddSingleton<JobCancellationService>();
        return services;
    }
}