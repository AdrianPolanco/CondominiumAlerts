using CondominiumAlerts.Domain.Aggregates.Interfaces;

namespace CondominiumAlerts.Domain.Aggregates.Entities
{
    public sealed class NotificationUser : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public bool IsSeen { get; set; }

        public Guid UserId { get; set; }

        public Guid NotificationId { get; set; }

        public User? User { get; set; }
        public Notification? Notification { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}