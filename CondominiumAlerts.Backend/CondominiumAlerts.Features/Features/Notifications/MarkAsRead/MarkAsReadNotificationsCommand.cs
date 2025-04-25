using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Notifications.MarkAsRead;

public record MarkAsReadNotificationsCommand(string UserId, List<Guid> NotificationIds): ICommand<Result<MarkAsReadNotificationCommandResponse>>;