using System.Text.Json;
using LightResults;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace CondominiumAlerts.CrossCutting.ErrorHandler;

public class ErrorHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Limpiar cualquier respuesta anterior
        httpContext.Response.Clear();
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        // Loggear la excepción usando Serilog
        Log.Error(exception, "An unhandled exception occurred while processing the request. Message: {Message}, StackTrace: {StackTrace}", exception.Message, exception.StackTrace);

        // Crear el objeto de error para la respuesta
        var error = new 
        {
            Success = false,
            Errors = new[] { exception.Message }
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(error, options);

        // Escribir la respuesta JSON con el error
        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }
}