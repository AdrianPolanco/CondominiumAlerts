using CondominiumAlerts.Domain.Aggregates.Interfaces;
using CondominiumAlerts.Domain.Aggregates.ValueObjects;

namespace CondominiumAlerts.Domain.Aggregates.Entities;

public class User : IAggregateRoot
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty!;
    public string LastName { get; set; } = string.Empty!;
    public Phone Phone { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public DateTime CreatedAt { get; init; }
}