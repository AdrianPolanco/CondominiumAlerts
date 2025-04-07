using System.Net;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Carter;
using CloudinaryDotNet;
using CondominiumAlerts.CrossCutting.ErrorHandler;
using CondominiumAlerts.Features.Features.Events;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using LightResults.Extensions.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace CondominiumAlerts.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();

        services.AddAuthentication();
        services.AddAuthorization();
        
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new ResultJsonConverterFactory());
        });
        
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile("./firebase.json")
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["FirebaseAuth:Authority"];
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["FirebaseAuth:Issuer"],
                    ValidAudience = configuration["FirebaseAuth:ProjectId"],
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var claims = new List<Claim>();
                    
                        if (context.Principal?.Identity is ClaimsIdentity identity)
                        {
                            // Mapear claims de Firebase a claims estándar
                            var userId = identity.FindFirst("user_id")?.Value 
                                         ?? identity.FindFirst("sub")?.Value;
                        
                            if (userId != null)
                            {
                                claims.Add(new Claim("user_id", userId));
                                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
                            }

                            var email = identity.FindFirst("email")?.Value;
                            if (email != null)
                            {
                                claims.Add(new Claim(ClaimTypes.Email, email));
                            }

                            // Agregar los claims al identity actual
                            identity.AddClaims(claims);
                        }
                    }
                };
            });

        services.AddScoped<Cloudinary>(sp => new Cloudinary(
            configuration.GetSection("Cloudinary").GetValue<string>("CLOUDINARY_URL")
        ));

        // Configura la política CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                policy.WithOrigins("http://localhost:4200")  // Origen permitido
                    .AllowAnyHeader()                   // Permitir cualquier encabezado
                    .AllowAnyMethod()                  // Permitir cualquier método (GET, POST, etc.)
                    .AllowCredentials();
            });
        });
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress;
                return RateLimitPartition.GetFixedWindowLimiter(ipAddress,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100, // Allow 100 requests
                        Window = TimeSpan.FromMinutes(1), // Per 1 minute window
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    })!;
            });
        });
        services.AddCarter();
        services.AddExceptionHandler<ErrorHandler>();
        services.AddTransient<EventNotificationJob>();
        
        return services;
    }

    public static ConfigureWebHostBuilder ConfigureHTTPVersion(this ConfigureWebHostBuilder hostBuilder, IConfiguration configuration)
    {
        hostBuilder.ConfigureKestrel(options =>
        {
            // HTTP/1.1
            options.ListenAnyIP(5000);
    
            // HTTPS with HTTP/1.1, HTTP/2 and HTTP/3
            options.ListenAnyIP(5001, listenOptions =>
            {
                listenOptions.UseHttps();
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
            });
        });
        
        return hostBuilder;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseExceptionHandler("/error");
        app.UseResponseCompression();
        app.UseRateLimiter();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapGroup("/api").MapCarter();
        app.UseCors(options =>
        {
            options.WithOrigins("http://localhost:4200") // Permite solicitudes desde este origen
                .AllowAnyMethod()                    // Permite cualquier método HTTP
                .AllowAnyHeader()                    // Permite cualquier cabecera
                .AllowCredentials();                 // Permite credenciales
        });
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        var scheduler = app.Services.GetRequiredService<IScheduler>();
        scheduler.Schedule<EventNotificationJob>().EveryMinute();
        return app;
    }
}