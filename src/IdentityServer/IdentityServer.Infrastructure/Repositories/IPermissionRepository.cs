﻿using Contracts.Common.Interfaces;
using IdentityServer.Infrastructure.Entities;

namespace IdentityServer.Infrastructure.Repositories;

public interface IPermissionRepository: IRepositoryBase<Permission,int>
{
    Task<IEnumerable<Permission>> GetPermissionsByRole(string roleId, bool trackChanges);
    void UpdatePermissionByRoleId(string roleId, IEnumerable<Permission> permissions, bool trackChanges);
}