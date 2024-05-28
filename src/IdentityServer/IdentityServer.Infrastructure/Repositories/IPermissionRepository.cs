using IdentityServer.Infrastructure.Domains;
using IdentityServer.Infrastructure.Entities;
using IdentityServer.Infrastructure.ViewModels;

namespace IdentityServer.Infrastructure.Repositories;

public interface IPermissionRepository: IIdentityRepositoryBase<Permission,int>
{
    Task<IReadOnlyList<PermissionViewModel>> GetPermissionsByRole(string roleId);
    void UpdatePermissionByRoleId(string roleId, IEnumerable<Permission> permissions, bool trackChanges);
}