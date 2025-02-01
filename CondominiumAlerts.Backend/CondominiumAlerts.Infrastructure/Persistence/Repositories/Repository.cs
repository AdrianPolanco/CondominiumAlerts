using CondominiumAlerts.Domain.Aggregates.Interfaces;
using CondominiumAlerts.Domain.Repositories;

namespace CondominiumAlerts.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
{
    public Task<List<TEntity>> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}