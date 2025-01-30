
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Hosting;

namespace CondominiumAlerts.CrossCutting.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCrossCuttingConcerns(this IServiceCollection services)
    {
        // Registrar el logger de Serilog
        //services.AddSerilogLogging(configuration);
        // Registrar el DiagnosticContext, que depende de ILogger
       // services.AddSingleton<DiagnosticContext>();

        return services;
    }
    
    public static void AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuración de Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)  // Lee la configuración de appsettings.json
            .Enrich.FromLogContext()               // Enriquecer los logs con el contexto
            .CreateLogger();

        // Registrar Serilog en el contenedor
        services.AddLogging(builder =>
        {
            builder.AddSerilog(); //Configurar Serilog como proveedor de logs
        });
    }

    public static WebApplication UseCrossCuttingConccerns(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}