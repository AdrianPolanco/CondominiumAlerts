using Carter;
using CondominiumAlerts.Features.Features.Condominium.Add;
using CondominiumAlerts.Features.Features.Condominium.Get;
using CondominiumAlerts.Features.Features.Condominium.GetCondominiumsJoinedByUser;
using CondominiumAlerts.Features.Features.Condominium.Join;
using LightResults;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CondominiumAlerts.Api.Endpoints
{
    public class CondominiumEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/condominium/join",
                async (ISender sender, [FromForm] JoinCondominiumCommand command, CancellationToken cancellationToken) =>
                {
                    Result<JoinCondominiumResponse> result = await sender.Send(command, cancellationToken);
                    if (!result.IsSuccess) return Results.BadRequest(result);

                    var responce = new
                    {
                        IsSuccess = result.IsSuccess,
                        Data = result.Value.Adapt<JoinCondominiumResponse>()
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

            app.MapGet("/condominium/GetById",
                async (ISender sender, [AsParameters] GetCondominiumCommand command, CancellationToken cancellationToken) =>
                {
                    Result<GetCondominiumResponce> result = await sender.Send(command, cancellationToken);

                    if (!result.IsSuccess) return Results.BadRequest(result);

                    var responce = new
                    {
                        IsSuccess = result.IsSuccess,
                        Data = result.Value,
                    };
                    return Results.Ok(responce);    
                });

            app.MapGet("/condominium/GetCondominiumsJoinedByUser",
                async (ISender sender, [AsParameters] GetCondominiumsJoinedByUserCommand command, CancellationToken cancellationToken) =>
                {
                    Result<List<GetCondominiumsJoinedByUserResponse>> result = await sender.Send(command, cancellationToken);

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
