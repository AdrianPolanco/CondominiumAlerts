using System.Net;
using System.Threading.RateLimiting;
using CondominiumAlerts.Api.Extensions;
using CondominiumAlerts.CrossCutting.Extensions;
using CondominiumAlerts.Features.Extensions;
using CondominiumAlerts.Infrastructure.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog();
});

builder.Host.UseSerilog();
builder.Logging.ClearProviders();

builder.Services.AddFeatures();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);
//builder.Services.AddCrossCuttingConcerns();

builder.WebHost.ConfigureHTTPVersion(builder.Configuration);



var app = builder.Build();

app.UseCrossCuttingConccerns();
app.UseApiServices();

app.Run();

app.Map("/error", (HttpContext httpContext) =>
{
    var exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
    if (exception != null)
    {
        Log.Error(exception, "Unhandled Exception: {Message}", exception.Message);
    }

    return Results.Problem(title: "An unexpected error occurred.");
});
