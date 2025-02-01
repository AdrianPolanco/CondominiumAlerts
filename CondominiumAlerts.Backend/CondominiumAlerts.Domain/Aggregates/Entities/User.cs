using CondominiumAlerts.Domain.Aggregates.Interfaces;
using CondominiumAlerts.Domain.Aggregates.ValueObjects;

namespace CondominiumAlerts.Domain.Aggregates.Entities;

public sealed class User : IAggregateRoot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty!;
    public string LastName { get; set; } = string.Empty!;
    public Phone Phone { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }


    public IReadOnlyCollection<Post>? Posts { get; set; }
    public IReadOnlyCollection<Comment>? Comments { get; set; }
    public IReadOnlyCollection<Message>? MessagesCreatedByUser { get; set; }
    public IReadOnlyCollection<Message>? MessagesReceiveByUser { get; set; }
    public IReadOnlyCollection<Notification>? NotificacionsReceiveByUser { get; set; }
    public IReadOnlyCollection<CondominiumUser>? Condominia { get; set; }
}