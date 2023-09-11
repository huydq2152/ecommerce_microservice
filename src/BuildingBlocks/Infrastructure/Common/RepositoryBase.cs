using System.Linq.Expressions;
using Contracts.Common.Interfaces;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Common;

public class RepositoryBase<T, K, TContext> : RepositoryQueryBase<T, K, TContext>,
    IRepositoryBase<T, K, TContext>
    where TContext : DbContext
    where T : EntityBase<K>
{
    private readonly TContext _dbContext;
    private readonly IUnitOfWork<TContext> _unitOfWork;

    public RepositoryBase(TContext dbContext, IUnitOfWork<TContext> unitOfWork) : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }


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