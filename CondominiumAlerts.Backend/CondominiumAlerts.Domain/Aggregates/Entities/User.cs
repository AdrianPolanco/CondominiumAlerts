using CondominiumAlerts.Domain.Aggregates.Interfaces;
using CondominiumAlerts.Domain.Aggregates.ValueObjects;

namespace CondominiumAlerts.Domain.Aggregates.Entities;

public sealed class User : IAggregateRoot<string>
{
    public string Id { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string Name { get; set; } = string.Empty!;
    public string Lastname { get; set; } = string.Empty!;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty!;
    public Address? Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public IReadOnlyCollection<Post>? Posts { get; set; }
    public IReadOnlyCollection<Comment>? Comments { get; set; }
    public IReadOnlyCollection<Message>? MessagesCreatedByUser { get; set; }
    public IReadOnlyCollection<Message>? MessagesReceivedByUser { get; set; }
    public IReadOnlyCollection<Notification>? NotificationsReceivedByUser { get; set; }
    public IReadOnlyCollection<Condominium>? Condominiums { get; set; }
    public IReadOnlyCollection<Summary> TriggeredSummaries { get; set; }
}