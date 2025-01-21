using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace CondominiumAlerts.CrossCutting.ErrorHandler;

public class ErrorHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}