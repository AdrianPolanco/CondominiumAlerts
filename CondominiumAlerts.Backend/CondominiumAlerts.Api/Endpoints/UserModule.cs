using System.Security.Claims;
using Carter;
using CondominiumAlerts.Features.Features.Users.Update;
using Mapster;
using MediatR;

namespace CondominiumAlerts.Api.Endpoints;

public class UserModule : ICarterModule
{
    private readonly ILogger<UserModule> _logger;

    public UserModule(ILogger<UserModule> logger)
    {
        _logger = logger;
    }
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        /*app.MapPut("/users/edit", async (ClaimsPrincipal claims, UpdateUserCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            
            var currentUserId = claims.FindFirst("user_id")?.Value;
            
            if (currentUserId == null) return Results.BadRequest("El id del usuario no existe.");
            var updateUserCommand = new UpdateUserCommand(currentUserId, command.Username, command.Name, command.Lastname, command.ProfilePic, command.Address);
            var result = await sender.Send(updateUserCommand, cancellationToken);
            if(result.IsFailed) return Results.BadRequest(result);

            var response = new
            {
                IsSuccess = result.IsSuccess,
                Data = result.Value.Adapt<UpdateUserResponse>()
            };
            
            return Results.Ok(response);
        }).RequireAuthorization();*/
    }
}