using CondominiumAlerts.Domain.Aggregates.Interfaces;
using CondominiumAlerts.Domain.Aggregates.ValueObjects;

namespace CondominiumAlerts.Domain.Aggregates.Entities;

public sealed class User : IAggregateRoot<string>
{
    public string Id { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string Name { get; set; } = string.Empty!;
    public string LastName { get; set; } = string.Empty!;
    public Phone Phone { get; set; } = null!;
    public Address? Address { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public IReadOnlyCollection<Post>? Posts { get; set; }
    public IReadOnlyCollection<Comment>? Comments { get; set; }
    public IReadOnlyCollection<Message>? MessagesCreatedByUser { get; set; }
    public IReadOnlyCollection<Message>? MessagesReceivedByUser { get; set; }
    public IReadOnlyCollection<Notification>? NotificationsReceivedByUser { get; set; }
    public IReadOnlyCollection<Condominium>? Condominiums { get; set; }
}