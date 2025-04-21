using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Events.GetByUser;

public record GetEventsBySubscriberQuery(string UserId): IQuery<Result<GetEventsBySubscriberQueryResponse>>;