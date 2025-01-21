using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Aggregates.Interfaces;

namespace CondominiumAlerts.Domain.Repositories;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    Task<List<TEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken);
    Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken);
    Task<TEntity> Delete(Guid id, CancellationToken cancellationToken);
}