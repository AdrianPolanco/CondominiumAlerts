using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;

namespace CondominiumAlerts.Features.Features.Notifications.MarkAsRead;

public class MarkAsReadNotificationsCommandHandler: ICommandHandler<MarkAsReadNotificationsCommand, Result<MarkAsReadNotificationCommandResponse>>
{
    private readonly IRepository<Notification, Guid> _repository;

    public MarkAsReadNotificationsCommandHandler(IRepository<Notification, Guid> repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<MarkAsReadNotificationCommandResponse>> Handle(MarkAsReadNotificationsCommand request, CancellationToken cancellationToken)
    {
        var readNotifications = await _repository.GetAsync(
            cancellationToken,
            filter: n => request.NotificationIds.Contains(n.Id)
        );
        
        readNotifications = readNotifications.Select(n =>
        {
            n.Read = true;
            return n;
        }).ToList();
        
        readNotifications = await _repository.BulkUpdateAsync(readNotifications, cancellationToken);
        
        var readNotificationsIds = readNotifications.Select(n => n.Id).ToList();

        return Result<MarkAsReadNotificationCommandResponse>.Ok(new(readNotificationsIds));
    }
}