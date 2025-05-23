﻿using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Carter;
using CondominiumAlerts.CrossCutting.ErrorHandler;
using CondominiumAlerts.Infrastructure.Persistence.Context;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using LightResults.Extensions.Json;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace CondominiumAlerts.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new ResultJsonConverterFactory());
        });
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile("./firebase.json")
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
        app.MapGroup("/api").MapCarter();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        return app;
    }
}