using IdentityServer.Infrastructure.Domains;
using IdentityServer.Infrastructure.Entities;

namespace IdentityServer.Infrastructure.Repositories;

public interface IPermissionRepository: IIdentityRepositoryBase<Permission,int>
{
    Task<IReadOnlyList<Permission>> GetPermissionsByRole(string roleId);
    void UpdatePermissionByRoleId(string roleId, IEnumerable<Permission> permissions, bool trackChanges);
}