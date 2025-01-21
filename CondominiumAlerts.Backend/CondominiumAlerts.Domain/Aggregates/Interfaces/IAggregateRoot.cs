namespace CondominiumAlerts.Domain.Aggregates.Interfaces;

public interface IAggregateRoot : IEntity
{
    public Guid Id { get; init; }
}