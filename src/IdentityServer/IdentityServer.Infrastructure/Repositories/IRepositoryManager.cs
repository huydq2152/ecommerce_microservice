using IdentityServer.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdentityServer.Infrastructure.Repositories;

public interface IRepositoryManager
{
    UserManager<User> UserManager { get; }
    RoleManager<IdentityRole> RoleManager { get; }
    IPermissionRepository Permission { get; }
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task EndTransactionAsync();
    void RollbackTransaction();
}