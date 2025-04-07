using System.Security.Claims;
using Carter;
using CondominiumAlerts.Features.Features.Events;
using CondominiumAlerts.Features.Features.Events.Create;
using CondominiumAlerts.Features.Features.Events.Delete;
using CondominiumAlerts.Features.Features.Events.Get;
using CondominiumAlerts.Features.Features.Events.Update;
using LightResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CondominiumAlerts.Api.Endpoints;

public class EventsModule: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/events",
            async (ISender Sender, CancellationToken cancellationToken, CreateEventCommand command) =>
            {
                Result<CreateEventResponse> result = await Sender.Send(command, cancellationToken);

                if (!result.IsSuccess)
                {
                    var failedResponse = new
                    {
                        IsSuccess = false,
                        Data = new
                        {
                            Message = result.Error.Message
                        }
                    };

                    return Results.BadRequest(failedResponse);
                }

                var successResponse = new
                {
                    IsSuccess = true,
                    Data = result.Value
                };

                return Results.Ok(successResponse);
            }) /*.RequireAuthorization()*/;

        app.MapPut("/events",
            async (
                ISender sender,
                CancellationToken cancellationToken,
                ClaimsPrincipal claims,
                UpdateEventCommand command) => // el parámetro complejo va al final
            {
                /*
                var requesterId = claims.FindFirst("user_id")?.Value;

                if (requesterId is null)
                {
                    var response = new
                    {
                        Success = false,
                        Data = new
                        {
                            Message = "No se proporcionó un token válido."
                        }
                    };

                    return Results.BadRequest(response);
                }

                command = command with { EditorId = requesterId };
                */

                var result = await sender.Send(command, cancellationToken);

                if (!result.IsSuccess)
                {
                    var response = new
                    {
                        Success = false,
                        Data = new
                        {
                            Message = result.Error.Message
                        }
                    };

                    return Results.BadRequest(response);
                }

                var successResponse = new
                {
                    IsSuccess = true,
                    Data = result.Value
                };

                return Results.Ok(successResponse);
            }) /*.RequireAuthorization()*/;

        app.MapGet("/events/{condominiumId}/user/{userId}",
            async (Guid condominiumId, string userId, ISender sender, CancellationToken cancellationToken,
                ClaimsPrincipal claims) =>
            {
                /*
                var requesterId = claims.FindFirst("user_id")?.Value;

                if (requesterId is null)
                {
                    var response = new
                    {
                        Success = false,
                        Data = new
                        {
                            Message = "No se proporcionó un token válido."
                        }
                    };

                    return Results.BadRequest(response);
                }
                */

                var query = new GetEventsQuery(condominiumId, userId, userId);

                var result = await sender.Send(query, cancellationToken);

                if (!result.IsSuccess)
                {
                    var response = new
                    {
                        Success = false,
                        Data = new
                        {
                            Message = result.Error.Message
                        }
                    };

                    return Results.BadRequest(response);
                }

                var successResponse = new
                {
                    IsSuccess = true,
                    Data = result.Value
                };

                return Results.Ok(successResponse);
            }
        ) /*.RequireAuthorization()*/;

        app.MapDelete("/events/{eventId}/user/{userId}",
            async (Guid eventId, string userId, ISender sender, CancellationToken cancellationToken, ClaimsPrincipal claims) =>
            {
                /*
               var requesterId = claims.FindFirst("user_id")?.Value;

               if (requesterId is null)
               {
                   var response = new
                   {
                       Success = false,
                       Data = new
                       {
                           Message = "No se proporcionó un token válido."
                       }
                   };

                   return Results.BadRequest(response);
               }
               */

                var query = new DeleteEventCommand(eventId, userId, userId);

                var result = await sender.Send(query, cancellationToken);

                if (!result.IsSuccess)
                {
                    var response = new
                    {
                        Success = false,
                        Data = new
                        {
                            Message = result.Error.Message
                        }
                    };

                    return Results.BadRequest(response);
                }
                
                var successResponse = new
                {
                    IsSuccess = true,
                    Data = result.Value
                };

                return Results.Ok(successResponse);
            })/*.RequireAuthorization()*/;

                app.MapHub<EventHub>("/hubs/events");
            }
}