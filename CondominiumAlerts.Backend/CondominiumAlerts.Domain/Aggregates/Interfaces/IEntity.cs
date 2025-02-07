namespace CondominiumAlerts.Domain.Aggregates.Interfaces;

public interface IEntity<TId>
{
    TId Id { get; set; }
    DateTime CreatedAt { get; init; }
    DateTime UpdatedAt { get; set; }
}