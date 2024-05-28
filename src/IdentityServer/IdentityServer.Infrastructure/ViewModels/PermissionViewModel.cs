using Contracts.Domains;

namespace IdentityServer.Infrastructure.ViewModels;

public class PermissionViewModel: EntityBase<int>
{
    public string RoleId { get; set; }

    public string Function { get; set; }

    public string Command { get; set; }
}