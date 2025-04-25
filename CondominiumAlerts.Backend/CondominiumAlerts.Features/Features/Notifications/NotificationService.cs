using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Interfaces;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Features.Notifications.Get;
using Mapster;
using Microsoft.AspNetCore.SignalR;

namespace CondominiumAlerts.Features.Features.Notifications
{
    internal class NotificationService
    : INotificationService
    {

        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(IHubContext<NotificationHub> notificationHubContext,
                                   INotificationRepository notificationRepository)
        {
            _notificationHubContext = notificationHubContext;
            _notificationRepository = notificationRepository;
        }

        public async Task Notify(Notification notification, string condominiumId, CancellationToken cancellationToken = default)
        {

            await _notificationRepository.CreateAsync(notification, cancellationToken);

            var notifcationWithLvl = await _notificationRepository.GetByIdAsync(
                notification.Id,
                cancellationToken,
                true,
                includes: [x => x.LevelOfPriority]
            )!;
            await _notificationHubContext.Clients.Group(condominiumId)
                .SendAsync(
                    NotificationHub.ReciveNotification,
                    notification.Adapt<NotificationDto>()
                );
        }
    }
}