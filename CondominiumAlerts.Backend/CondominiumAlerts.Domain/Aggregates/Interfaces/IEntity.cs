namespace CondominiumAlerts.Domain.Aggregates.Interfaces;

public interface IEntity<TId> : IBaseEntity<TId>
{
    DateTime UpdatedAt { get; set; }
}