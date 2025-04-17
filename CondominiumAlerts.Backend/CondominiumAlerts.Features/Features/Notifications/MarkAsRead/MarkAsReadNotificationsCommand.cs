using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Notifications.MarkAsRead;

public record MarkAsReadNotificationsCommand(List<Guid> NotificationIds): ICommand<Result<MarkAsReadNotificationCommandResponse>>;