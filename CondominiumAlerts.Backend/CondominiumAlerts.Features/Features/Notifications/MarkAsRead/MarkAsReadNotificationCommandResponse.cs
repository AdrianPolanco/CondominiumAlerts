﻿namespace CondominiumAlerts.Features.Features.Notifications.MarkAsRead;

public record MarkAsReadNotificationCommandResponse(List<Guid> MarkedAsReadNotificationsIds);