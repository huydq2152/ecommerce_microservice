using Microsoft.EntityFrameworkCore;

namespace Contracts.Common.Interfaces;

public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
{
    Task<int> CommitAsync();
}