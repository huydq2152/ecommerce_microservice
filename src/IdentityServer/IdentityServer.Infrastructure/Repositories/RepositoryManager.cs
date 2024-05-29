using AutoMapper;
using Contracts.Common.Interfaces;
using IdentityServer.Infrastructure.Entities;
using IdentityServer.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdentityServer.Infrastructure.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly IUnitOfWork<IdentityContext> _unitOfWork;
    private readonly IdentityContext _identityContext;
    private readonly Lazy<IPermissionRepository> _permissionRepository;
    private readonly IMapper _mapper;

    public RepositoryManager(IUnitOfWork<IdentityContext> unitOfWork, IdentityContext identityContext,
        UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _identityContext = identityContext;
        UserManager = userManager;
        RoleManager = roleManager;
        _mapper = mapper;
        _permissionRepository =
            new Lazy<IPermissionRepository>(() =>
                new PermissionRepository(_identityContext, _unitOfWork, userManager, mapper));
    }

    public UserManager<User> UserManager { get; }
    public RoleManager<IdentityRole> RoleManager { get; }
    public IPermissionRepository Permission => _permissionRepository.Value;

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