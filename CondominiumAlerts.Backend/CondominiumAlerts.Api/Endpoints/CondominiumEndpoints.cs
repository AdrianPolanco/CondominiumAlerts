using Carter;
using CondominiumAlerts.Features.Features.Condominium;
using CondominiumAlerts.Features.Features.Condominium.Join;
using LightResults;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CondominiumAlerts.Api.Endpoints
{
    public class CondominiumEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/condominium/join",
                async (ISender sender, JoinCondominiumCommand command, CancellationToken cancellationToken) =>
                {
                    Result<JoinCondominiumResponce> result = await sender.Send(command, cancellationToken);
                    if (!result.IsSuccess) return Results.BadRequest(result);

                    var responce = new
                    {
                        IsSuccess = result.IsSuccess,
                        Data = result.Value.Adapt<JoinCondominiumResponce>()
                    };
                    return Results.Ok(responce);
                });
        }
    }
}
