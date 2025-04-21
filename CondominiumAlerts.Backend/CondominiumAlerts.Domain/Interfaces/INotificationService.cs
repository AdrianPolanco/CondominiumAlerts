using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Domain.Interfaces
{
    public interface INotificationService
    {
        public Task Notify(Notification notification, string condominiumId, CancellationToken cancellationToken = default);
    }
}