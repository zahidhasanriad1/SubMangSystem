using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Domain.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AssignFlow.DataAccess.Repositories;

public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class
{
    protected readonly AppDbContext DbContext;
    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    public Repository(AppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public async Task<ICollection<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<ICollection<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return await DbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<int> AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
        return await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        return await DbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<int> UpdateRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default)
    {
        DbSet.UpdateRange(entities);
        return await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return await DbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
