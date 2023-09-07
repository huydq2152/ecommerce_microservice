using System.Linq.Expressions;
using Contracts.Common.Interfaces;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Common;

public class RepositoryBaseAsync<T, K, TContext> : IRepositoryBaseAsync<T, K, TContext>
    where TContext : DbContext
    where T : EntityBase<K>
{
    private readonly TContext _dbContext;
    private readonly IUnitOfWork<TContext> _unitOfWork;


    public RepositoryBaseAsync(TContext dbContext, IUnitOfWork<TContext> unitOfWork)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public IQueryable<T> FindAll(bool trackChanges = false) =>
        !trackChanges ? _dbContext.Set<T>().AsNoTracking() : _dbContext.Set<T>();

    public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindAll(trackChanges);
        items = includeProperties.Aggregate(items, (current, includeProperties) => current.Include(includeProperties));
        return items;
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false) =>
        !trackChanges ? _dbContext.Set<T>().Where(expression).AsNoTracking() : _dbContext.Set<T>().Where(expression);

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindByCondition(expression, trackChanges);
        items = includeProperties.Aggregate(items, (current, includeProperties) => current.Include(includeProperties));
        return items;
    }

    public async Task<T> GetByIdAsync(K id) => await FindByCondition(o => o.Id.Equals(id)).FirstOrDefaultAsync();

    public async Task<T> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties) =>
        await FindByCondition(o => o.Id.Equals(id), trackChanges: false, includeProperties).FirstOrDefaultAsync();

    public Task<IDbContextTransaction> BeginTransactionAsync() => _dbContext.Database.BeginTransactionAsync();

    public async Task EndTransactionAsync()
    {
        await SaveChangeAsync();
        await _dbContext.Database.CommitTransactionAsync();
    }

    public Task RollbackTransactionAsync() => _dbContext.Database.RollbackTransactionAsync();

    public async Task<K> CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        return entity.Id;
    }

    public async Task<IList<K>> CreateListAsync(IEnumerable<T> entities)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
        return entities.Select(o => o.Id).ToList();
    }

    public Task UpdateAsync(T entity)
    {
        T exist = _dbContext.Set<T>().Find(entity.Id);
        _dbContext.Entry(exist).CurrentValues.SetValues(entity);
        return Task.CompletedTask;
    }

    public Task UpdateListAsync(IEnumerable<T> entities)
    {
        return _dbContext.Set<T>().AddRangeAsync(entities);
    }

    public Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteListAsync(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangeAsync()
    {
        return _unitOfWork.CommitAsync();
    }
}