using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.CrossCutting.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var uniqueId = Guid.NewGuid().ToString();

        logger.LogInformation(
            "Beginning Request {RequestName} [{UniqueId}] {@Request}",
            requestName, uniqueId, request);

        var timer = Stopwatch.StartNew();

        try
        {
            var response = await next();
            timer.Stop();

            logger.LogInformation(
                "Completed Request {RequestName} [{UniqueId}] in {ElapsedMilliseconds}ms {@Response}",
                requestName, uniqueId, timer.ElapsedMilliseconds, response);

            return response;
        }
        catch (Exception ex)
        {
            timer.Stop();
            logger.LogError(ex, 
                "Error in Request {RequestName} [{UniqueId}] after {ElapsedMilliseconds}ms", 
                requestName, uniqueId, timer.ElapsedMilliseconds);

            throw; // Asegura que el flujo sigue sin que ASP.NET Core lo relance nuevamente
        }
    }
}