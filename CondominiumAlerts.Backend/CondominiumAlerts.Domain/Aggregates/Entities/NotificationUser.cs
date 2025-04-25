using CondominiumAlerts.Domain.Aggregates.Interfaces;

namespace CondominiumAlerts.Domain.Aggregates.Entities
{
    public sealed class NotificationUser : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public bool Read { get; set; }

        public string UserId { get; set; } = string.Empty;

        public Guid NotificationId { get; set; }

        public User? User { get; set; }
        public Notification? Notification { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}