using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Notifications.Get;

public record GetEventRelatedNotificationsQuery(string UserId, string RequesterId): IQuery<Result<GetEventRelatedNotificationsQueryResponse>>;