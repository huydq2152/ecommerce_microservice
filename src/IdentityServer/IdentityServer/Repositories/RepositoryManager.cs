using Contracts.Common.Interfaces;
using IdentityServer.Entities;
using IdentityServer.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdentityServer.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly IUnitOfWork<IdentityContext> _unitOfWork;
    private readonly IdentityContext _identityContext;

    public RepositoryManager(IUnitOfWork<IdentityContext> unitOfWork, IdentityContext identityContext,
        UserManager<User> userManager, RoleManager<User> roleManager)
    {
        _unitOfWork = unitOfWork;
        _identityContext = identityContext;
        UserManager = userManager;
        RoleManager = roleManager;
    }

    public UserManager<User> UserManager { get; }
    public RoleManager<User> RoleManager { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _unitOfWork.CommitAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _identityContext.Database.BeginTransactionAsync();
    }

    public async Task EndTransactionAsync()
    {
        await _identityContext.Database.CommitTransactionAsync();
    }

    public void RollbackTransaction()
    {
        _identityContext.Database.RollbackTransaction();
    }
}