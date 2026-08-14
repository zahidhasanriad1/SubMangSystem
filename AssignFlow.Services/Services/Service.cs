using AssignFlow.DataAccess.Interfaces;
using AssignFlow.Services.Interfaces;
using AssignFlow.Utils.Exceptions;
using System.Linq.Expressions;

namespace AssignFlow.Services.Services;

public class Service<TEntity, TId> : IService<TEntity, TId> where TEntity : class
{
    private readonly IRepository<TEntity, TId> _repository;

    public Service(IRepository<TEntity, TId> repository)
    {
        _repository = repository;
    }

    public virtual async Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity ?? throw new NotFoundException($"{typeof(TEntity).Name} was not found.");
    }

    public Task<ICollection<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return _repository.ListAsync(cancellationToken);
    }

    public Task<ICollection<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _repository.ListAsync(predicate, cancellationToken);
    }

    public Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetAsync(predicate, cancellationToken);
    }

    public Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return _repository.AddAsync(entity, cancellationToken);
    }

    public Task<int> AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return _repository.AddRangeAsync(entities, cancellationToken);
    }

    public Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(entity, cancellationToken);
    }

    public Task<int> UpdateRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateRangeAsync(entities, cancellationToken);
    }

    public async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        return await _repository.DeleteAsync(entity, cancellationToken);
    }
}
