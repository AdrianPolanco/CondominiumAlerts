using CondominiumAlerts.Domain.Aggregates.Interfaces;
using CondominiumAlerts.Domain.Repositories;

namespace CondominiumAlerts.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    public Task<List<TEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> Delete(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}