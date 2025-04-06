using Carter;
using CondominiumAlerts.Features.Events;
using CondominiumAlerts.Features.Events.Create;
using LightResults;
using MediatR;

namespace CondominiumAlerts.Api.Endpoints;

public class EventsModule: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/events", async (ISender Sender, CancellationToken cancellationToken, CreateEventCommand command) =>
        {
            Result<CreateEventResponse> result = await Sender.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                var failedResponse = new
                {
                    IsSuccess = false,
                    Data = new {
                        Message = result.Value
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
        })/*.RequireAuthorization()*/;

        app.MapHub<EventHub>("/hubs/events");
    }
}