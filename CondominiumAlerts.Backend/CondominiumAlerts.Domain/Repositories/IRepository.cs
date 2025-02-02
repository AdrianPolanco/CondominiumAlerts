using System.Linq.Expressions;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Aggregates.Interfaces;

namespace CondominiumAlerts.Domain.Repositories;

public interface IRepository<TEntity> where TEntity : class, IEntity<Guid>
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool readOnly = false, bool ignoreQueryFilters = false, params Expression<Func<TEntity, object>>[] includes);

    Task<List<TEntity>> GetAsync(CancellationToken cancellationToken, Expression<Func<TEntity, bool>>? filter = null,
        bool readOnly = true, bool ignoreQueryFilters = false, Expression<Func<TEntity, object>>[]? includes = null);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<List<TEntity>> BulkInsertAsync(List<TEntity> entities, CancellationToken cancellationToken);
    Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<List<TEntity>> BulkUpdateAsync(List<TEntity> entities, CancellationToken cancellationToken);
    Task<TEntity?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> BulkDeleteAsync(List<TEntity> entities, CancellationToken cancellationToken);
}