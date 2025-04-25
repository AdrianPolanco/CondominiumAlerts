using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Features.Features.Notifications.Get;

public record GetEventRelatedNotificationsQueryResponse(List<NotificationDto> Notifications);