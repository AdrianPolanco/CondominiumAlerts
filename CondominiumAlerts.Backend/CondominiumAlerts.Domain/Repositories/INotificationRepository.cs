using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Domain.Repositories
{
    public interface INotificationRepository
    : IRepository<Notification, Guid>
    {
    }
}