using Contracts.Common.Interfaces;
using Dapper;
using IdentityServer.Infrastructure.Domains;
using IdentityServer.Infrastructure.Entities;
using IdentityServer.Infrastructure.ViewModels;
using IdentityServer.Persistence;

namespace IdentityServer.Infrastructure.Repositories;

public class PermissionRepository: IdentityRepositoryBase<Permission, int>, IPermissionRepository 
{
    public PermissionRepository(IdentityContext dbContext, IUnitOfWork<IdentityContext> unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IReadOnlyList<PermissionViewModel>> GetPermissionsByRole(string roleId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId);
        var result = await QueryAsync<PermissionViewModel>(
            "Get_Permission_ByRoleId",
            parameters);

        return result;
    }

    public void UpdatePermissionByRoleId(string roleId, IEnumerable<Permission> permissions, bool trackChanges)
    {
        throw new NotImplementedException();
    }
}