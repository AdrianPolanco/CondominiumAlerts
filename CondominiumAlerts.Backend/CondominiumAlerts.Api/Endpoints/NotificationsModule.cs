using System.Security.Claims;
using Carter;
using CondominiumAlerts.Features.Features.Events.Get;
using CondominiumAlerts.Features.Features.Notifications.Get;
using CondominiumAlerts.Features.Features.Notifications.MarkAsRead;
using MediatR;

namespace CondominiumAlerts.Api.Endpoints;

public class NotificationsModule: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/notifications/user/{userId}",
            async (string userId, ISender sender, CancellationToken cancellationToken,
                ClaimsPrincipal claims) =>
            {
                
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

                var query = new GetEventRelatedNotificationsQuery(userId, requesterId);

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

                    return Results.Unauthorized();
                }

                var successResponse = new
                {
                    IsSuccess = true,
                    Data = result.Value
                };

                return Results.Ok(successResponse);
            }
        ).RequireAuthorization();
        
        app.MapPut("/notifications/read",
            async (List<Guid> NotificationsIds, ISender sender, CancellationToken cancellationToken,
                ClaimsPrincipal claims) =>
            {
                
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

                var query = new MarkAsReadNotificationsCommand(NotificationsIds);

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
        ).RequireAuthorization();
    }
}