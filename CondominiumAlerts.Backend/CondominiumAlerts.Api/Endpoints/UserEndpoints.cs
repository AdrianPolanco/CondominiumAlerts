using Carter;
using CondominiumAlerts.Features.Features.Users.GetCondominiumsUsers;
using LightResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CondominiumAlerts.Api.Endpoints
{
    public class UserEndpoints : ICarterModule
    {
        
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/user/GetCondominiumUsers",
                async ( ISender sender, [FromBody] GetCondominiumsUsersCommand request, CancellationToken cancellationToken ) =>
                {
                    Result<List<GetCondominiumsUsersResponse>> result = await sender.Send(request, cancellationToken);

                    if (!result.IsSuccess) return Results.BadRequest(result);

                    var responce = new
                    {
                        IsSuccess = result.IsSuccess,
                        Data = result.Value,
                    };

                    return Results.Ok(responce);
                });
        }
    }
}
