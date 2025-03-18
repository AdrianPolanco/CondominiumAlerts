using System.Linq;
using System.Linq.Expressions;
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Persistence.Context;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CondominiumAlerts.Infrastructure.Persistence.Repositories;

public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IBaseEntity<TId>
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken)
    {
       return _dbSet.AnyAsync(filter, cancellationToken);
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken, bool readOnly = false, bool ignoreQueryFilters = false, params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            if (ignoreQueryFilters) query = query.IgnoreQueryFilters();
            // Aplicar includes opcionales
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (readOnly) query = query.AsNoTracking();


            TEntity? entity = await query.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
            return entity;
        }
        catch
        {
            return null;
        }
    }

    public virtual async Task<List<TEntity>> GetAsync(CancellationToken cancellationToken, Expression<Func<TEntity, bool>>? filter = null, bool readOnly = true, bool ignoreQueryFilters = false, Expression<Func<TEntity, object>>[]? includes = null)
    {
        IQueryable<TEntity> query = _dbSet.AsQueryable();

        if (ignoreQueryFilters) query = query.IgnoreQueryFilters();

        if (filter is not null) query = query.Where(filter);

        if (includes != null)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        if (readOnly) query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }
   public async Task<TEntity?> GetOneByFilterAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default, bool readOnly = true, bool ignoreQueryFilters = false, Expression<Func<TEntity, object>>[]? includes = null)
    {
        IQueryable<TEntity> query = _dbSet.AsQueryable();

        if (readOnly) query.AsQueryable();

        if(ignoreQueryFilters) query = query.IgnoreQueryFilters();

        if (includes != null) query = includes.Aggregate(query, (curremt, include) => curremt.Include(include));

        return await query.FirstOrDefaultAsync(filter,cancellationToken);
        
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        using (await BeginTransactionAsync())
        {
            try
            {
                entity.CreatedAt = DateTime.UtcNow;
                await _dbSet.AddAsync(entity, cancellationToken);
                await SaveChangesAsync(cancellationToken);
                await CommitAsync(cancellationToken);
                return entity;
            }
            catch (Exception)
            {
                await RollbackAsync(cancellationToken);
                throw;
            }
        }
    }

    public async Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        using (await BeginTransactionAsync())
        {
            try
            {
                if(entity is IEntity<TId> updatableEntity) {
                    updatableEntity.UpdatedAt = DateTime.UtcNow;
                }
                _dbSet.Update(entity);
                await SaveChangesAsync(cancellationToken);
                await CommitAsync(cancellationToken);
                return entity;
            }
            catch
            {
                await RollbackAsync(cancellationToken);
                throw;
            }
        }
    }

    public async Task<TEntity?> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        using (await BeginTransactionAsync())
        {
            try
            {
                var entity = await GetByIdAsync(id, cancellationToken, ignoreQueryFilters: true);
                if (entity is null) return null;
                _dbSet.Remove(entity);
                await SaveChangesAsync(cancellationToken);
                await CommitAsync(cancellationToken);
                return entity;
            }
            catch
            {
                await RollbackAsync(cancellationToken);
                throw;
            }
        }
    }



    public async Task<List<TEntity>> BulkInsertAsync(List<TEntity> entities, CancellationToken cancellationToken)
    {
        try
        {
            await _context.BulkInsertAsync(entities, cancellationToken: cancellationToken);
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }

        return entities;
    }

    public async Task<List<TEntity>> BulkDeleteAsync(List<TEntity> entities, CancellationToken cancellationToken)
    {
        try
        {
            await _context.BulkDeleteAsync(entities, cancellationToken: cancellationToken);
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }


        return entities;
    }

    public async Task<List<TEntity>> BulkUpdateAsync(List<TEntity> entities, CancellationToken cancellationToken)
    {
        try
        {
            if (entities is List<IEntity<TId>> updatableEntities)
            {
                updatableEntities.ForEach(e => e.UpdatedAt = DateTime.Now);
            }
            await _context.BulkUpdateAsync(entities, cancellationToken: cancellationToken);
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }

        return entities;
    }

 

    protected async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
        }
    }

    protected async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    protected async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _context.Database.CommitTransactionAsync(cancellationToken);
    }

    protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}