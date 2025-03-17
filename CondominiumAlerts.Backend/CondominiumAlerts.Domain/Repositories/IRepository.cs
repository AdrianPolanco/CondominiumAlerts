using System.Linq.Expressions;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Aggregates.Interfaces;

namespace CondominiumAlerts.Domain.Repositories;

public interface IRepository<TEntity, TId> where TEntity : class, IBaseEntity<TId>
{
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken, bool readOnly = false, bool ignoreQueryFilters = false, params Expression<Func<TEntity, object>>[] includes);

    Task<List<TEntity>> GetAsync(CancellationToken cancellationToken, Expression<Func<TEntity, bool>>? filter = null,
        bool readOnly = true, bool ignoreQueryFilters = false, Expression<Func<TEntity, object>>[]? includes = null);
    Task<TEntity?> GetOneByFilterAsync( Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default,
        bool readOnly = true, bool ignoreQueryFilters = false, Expression<Func<TEntity, object>>[]? includes = null);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<List<TEntity>> BulkInsertAsync(List<TEntity> entities, CancellationToken cancellationToken);
    Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<List<TEntity>> BulkUpdateAsync(List<TEntity> entities, CancellationToken cancellationToken);
    Task<TEntity?> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> BulkDeleteAsync(List<TEntity> entities, CancellationToken cancellationToken);
}