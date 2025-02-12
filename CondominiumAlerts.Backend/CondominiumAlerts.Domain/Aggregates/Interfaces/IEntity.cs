namespace CondominiumAlerts.Domain.Aggregates.Interfaces;

public interface IEntity<TId>
{
    TId Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}