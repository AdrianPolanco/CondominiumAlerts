using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;

namespace CondominiumAlerts.Features.Features.Notifications.MarkAsRead;

public class MarkAsReadNotificationsCommandHandler : ICommandHandler<MarkAsReadNotificationsCommand, Result<MarkAsReadNotificationCommandResponse>>
{
    private readonly IRepository<Notification, Guid> _notificationRepository;
    private readonly IRepository<NotificationUser, Guid> _notificationUserRepository;

    public MarkAsReadNotificationsCommandHandler(IRepository<Notification, Guid> notificationRepository, IRepository<NotificationUser, Guid> notificationUserRepository)
    {
        _notificationRepository = notificationRepository;
        _notificationUserRepository = notificationUserRepository;
    }

    public async Task<Result<MarkAsReadNotificationCommandResponse>> Handle(MarkAsReadNotificationsCommand request, CancellationToken cancellationToken)
    {
        var readNotificationsIds = new List<Guid>();
        var notis = await _notificationUserRepository.GetAsync(

            cancellationToken,
            filter: n => request.NotificationIds.Contains(n.NotificationId)
        );
        notis.ForEach(n => n.Read = true);

        readNotificationsIds.AddRange(
            (await _notificationUserRepository.BulkUpdateAsync(
                notis,
                cancellationToken
            )).ConvertAll(n => n.NotificationId)
        );

        var notisUsers = await _notificationUserRepository.BulkInsertAsync(
            (from noti in request.NotificationIds.Except(readNotificationsIds)
             select new NotificationUser()
             {
                 Id = Guid.NewGuid(),
                 NotificationId = noti,
                 UserId = request.UserId,
                 Read = true
             }).ToList(),
            cancellationToken
        );
        readNotificationsIds.AddRange(
            notisUsers.Select(x => x.NotificationId)
        );

        return Result<MarkAsReadNotificationCommandResponse>.Ok(new(readNotificationsIds));
    }
}