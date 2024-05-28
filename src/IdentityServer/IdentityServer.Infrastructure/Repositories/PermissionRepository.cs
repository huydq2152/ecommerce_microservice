using Contracts.Common.Interfaces;
using IdentityServer.Infrastructure.Domains;
using IdentityServer.Infrastructure.Entities;
using IdentityServer.Persistence;

namespace IdentityServer.Infrastructure.Repositories;

public class PermissionRepository: IdentityRepositoryBase<Permission, int>, IPermissionRepository 
{
    public PermissionRepository(IdentityContext dbContext, IUnitOfWork<IdentityContext> unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IReadOnlyList<Permission>> GetPermissionsByRole(string roleId)
    {
        throw new NotImplementedException();
    }

    public void UpdatePermissionByRoleId(string roleId, IEnumerable<Permission> permissions, bool trackChanges)
    {
        throw new NotImplementedException();
    }
}