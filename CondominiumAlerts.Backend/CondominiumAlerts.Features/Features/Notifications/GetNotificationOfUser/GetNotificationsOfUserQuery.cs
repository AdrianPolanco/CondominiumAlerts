using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using CondominiumAlerts.Features.Features.Messages;
using LightResults;

namespace CondominiumAlerts.Features.Features.Notifications.Get
{
    public record GetNotificationsOfUserQuery(
        string UserId,
        string RequesterId
    ) : IQuery<Result<List<NotificationDto>>>;
}
