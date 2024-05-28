using Contracts.Common.Interfaces;
using IdentityServer.Entities;

namespace IdentityServer.Common.Repositories;

public interface IPermissionRepository: IRepositoryBase<Permission,int>
{
    Task<IEnumerable<Permission>> GetPermissionsByRole(string roleId, bool trackChanges);
    void UpdatePermissionByRoleId(string roleId, IEnumerable<Permission> permissions, bool trackChanges);
}