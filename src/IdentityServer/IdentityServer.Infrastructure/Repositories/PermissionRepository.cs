using Contracts.Common.Interfaces;
using IdentityServer.Infrastructure.Entities;
using IdentityServer.Persistence;
using Infrastructure.Common.Repositories;

namespace IdentityServer.Infrastructure.Repositories;

public class PermissionRepository: RepositoryBase<Permission, int, IdentityContext>, IPermissionRepository 
{
    public PermissionRepository(IdentityContext dbContext, IUnitOfWork<IdentityContext> unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByRole(string roleId, bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public void UpdatePermissionByRoleId(string roleId, IEnumerable<Permission> permissions, bool trackChanges)
    {
        throw new NotImplementedException();
    }
}