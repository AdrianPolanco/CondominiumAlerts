
using Carter;
using CondominiumAlerts.Api.Hubs;
using CondominiumAlerts.Features.Features.Notifications;
using MediatR;
using CondominiumAlerts.Features.Features.Notifications.Get;

namespace CondominiumAlerts.Api.Endpoints
{
    public class NotificationModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapHub<NotificationHub>("/hubs/notifications");
            app.MapGet("/user/notifications", async (ISender sender,
                                      [AsParameters] GetNotificationsOfUserQuery query) =>
           {
               LightResults.Result<List<NotificationDto>> result = await sender.Send(query);

               if (!result.IsSuccess) return Results.BadRequest(result);

               var response = new
               {
                   IsSuccess = result.IsSuccess,
                   Data = result.Value,
               };
               return Results.Ok(response);

           });
        }
    }
}