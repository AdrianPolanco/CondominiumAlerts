using Carter;
using CondominiumAlerts.Features.Features.Condominium.Add;
using CondominiumAlerts.Features.Features.Condominium.Join;
using FirebaseAdmin.Auth;
using LightResults;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace CondominiumAlerts.Api.Endpoints
{
    public class CondominiumModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/condominium/join",
                async (ISender sender, [FromForm] JoinCondominiumCommand command, CancellationToken cancellationToken) =>
                {
                    Result<JoinCondominiumResponce> result = await sender.Send(command, cancellationToken);
                    if (!result.IsSuccess) return Results.BadRequest(result);

                    var responce = new
                    {
                        IsSuccess = result.IsSuccess,
                        Data = result.Value.Adapt<JoinCondominiumResponce>()
                    };
                    return Results.Ok(responce);
                }).DisableAntiforgery();

            app.MapPost("/condominium",
                async (ISender sender, [FromForm] AddCondominiumCommand command,CancellationToken cancellationToken) =>
                {
              
                    Result<AddCondominiumResponse> result = await sender.Send(command, cancellationToken);
                    if (!result.IsSuccess) return Results.BadRequest(result);

                    var response = new
                    {
                        result.IsSuccess,
                        Data = result.Value
                    };
                    return Results.Ok(response);
                }
                // TODO: Add anti forgery token in frontend: https://stackoverflow.com/a/77191406
            ).DisableAntiforgery();
        }
    }
}
