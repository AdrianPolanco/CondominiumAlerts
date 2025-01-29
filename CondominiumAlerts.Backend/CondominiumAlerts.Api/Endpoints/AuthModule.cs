using Carter;
using CondominiumAlerts.CrossCutting.Results;
using CondominiumAlerts.Features.Commands;
using MediatR;

namespace CondominiumAlerts.Api.Endpoints;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/register",
            async (ISender sender, RegisterUserCommand registerUserCommand, CancellationToken cancellationToken) =>
            {
               CustomResult<object> result = await sender.Send(registerUserCommand, cancellationToken);
               return result.ToHttpResult();
            });
    }
}