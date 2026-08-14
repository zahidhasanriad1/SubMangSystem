using System.Linq.Expressions;

namespace AssignFlow.Services.Interfaces;

public interface IService<TEntity, TId> where TEntity : class
{
    Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<ICollection<TEntity>> ListAsync(CancellationToken cancellationToken = default);
    Task<ICollection<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<int> AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<int> UpdateRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
}
