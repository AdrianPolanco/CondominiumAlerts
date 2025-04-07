using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Persistence.Context;

namespace CondominiumAlerts.Infrastructure.Persistence.Repositories
{
    internal class NotificationRepository
    : Repository<Notification, Guid>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context)
        : base(context)
        {
        }
    }
}