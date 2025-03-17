namespace CondominiumAlerts.Domain.Aggregates.Interfaces;

public interface IBaseEntity<TId>
{
    TId Id { get; set; }
    DateTime CreatedAt { get; set; }
}