using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Events.Get;

public record GetEventsQuery(Guid CondominiumId, string UserId, string RequesterId): IQuery<Result<List<GetEventsQueryResponse>>>;