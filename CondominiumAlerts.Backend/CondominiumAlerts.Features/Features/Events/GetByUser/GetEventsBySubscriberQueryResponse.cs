using CondominiumAlerts.Features.Features.Events.Get;

namespace CondominiumAlerts.Features.Features.Events.GetByUser;

public record GetEventsBySubscriberQueryResponse(List<GetEventsQueryResponse> Events);