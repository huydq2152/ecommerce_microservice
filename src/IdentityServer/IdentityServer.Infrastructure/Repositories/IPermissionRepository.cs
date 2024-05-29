using IdentityServer.Infrastructure.Domains;
using IdentityServer.Infrastructure.Entities;
using IdentityServer.Infrastructure.ViewModels;

namespace IdentityServer.Infrastructure.Repositories;

public interface IPermissionRepository : IIdentityRepositoryBase<Permission, int>
{
    Task<IReadOnlyList<PermissionViewModel>> GetPermissionsByRole(string roleId);
    Task<PermissionViewModel?> CreatePermission(string roleId, PermissionAddModel model);
    Task UpdatePermissionsByRoleId(string roleId, IEnumerable<PermissionAddModel> permissionCollection);
    Task DeletePermission(string roleId, string function, string command);
    Task<IEnumerable<PermissionUserViewModel>> GetPermissionsByUser(User user);
}