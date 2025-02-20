using Carter;

using CondominiumAlerts.Features.Features.Users.Register;
using Mapster;
using MediatR;

namespace CondominiumAlerts.Api.Endpoints;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/register",
            async (ISender sender, RegisterUserCommand registerUserCommand, CancellationToken cancellationToken) =>
            {
               var result = await sender.Send(registerUserCommand, cancellationToken);
               if(result.IsFailed) return Results.BadRequest(result);
               var response = new
               {
                   IsSuccess = result.IsSuccess,
                   Data = result.Value.Adapt<RegisterUserResponse>()
               };
               return Results.Ok(response);
            });
        
        app.MapGet("/hello", ()=> Results.Ok("Hello!"));
    }
}