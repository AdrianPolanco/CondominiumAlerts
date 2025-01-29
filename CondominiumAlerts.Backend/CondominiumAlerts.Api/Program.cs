using System.Net;
using System.Threading.RateLimiting;
using CondominiumAlerts.Api.Extensions;
using CondominiumAlerts.Features.Extensions;
using CondominiumAlerts.Infrastructure.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddFeatures();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseApiServices();

app.Run();