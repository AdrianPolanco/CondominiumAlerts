
using Carter;
using CondominiumAlerts.Api.Hubs;
using CondominiumAlerts.Features.Features.Notifications;
using MediatR;
using CondominiumAlerts.Features.Features.Notifications.Get;
using System.Security.Claims;
using CondominiumAlerts.Features.Features.Notifications.MarkAsRead;
using LightResults;

namespace CondominiumAlerts.Api.Endpoints
{
    public class NotificationModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapHub<NotificationHub>("/hubs/notifications");

            app.MapGet("/notifications/user/{userId}",
                async (ISender sender,
                       string userId,
                       ClaimsPrincipal claims
            ) =>
           {
               var requesterId = claims.FindFirst("user_id")?.Value;

               if (requesterId is null)
               {
                   return Results.BadRequest(new
                   {
                       Success = false,
                       Data = new
                       {
                           Message = "No se proporcionó un token válido."
                       }
                   });
               }
               Result<List<NotificationDto>> result
                   = await sender.Send(
                       new GetNotificationsOfUserQuery(userId, requesterId)
                   );

               if (!result.IsSuccess) return Results.BadRequest(result);

               var response = new
               {
                   IsSuccess = result.IsSuccess,
                   Data = result.Value,
               };
               return Results.Ok(response);

           }).RequireAuthorization();

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
}