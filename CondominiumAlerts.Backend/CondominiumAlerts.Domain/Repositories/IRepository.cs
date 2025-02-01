using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Aggregates.Interfaces;

namespace CondominiumAlerts.Domain.Repositories;

public interface IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
{
    Task<List<TEntity>> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity> DeleteAsync(TId id, CancellationToken cancellationToken = default);
}