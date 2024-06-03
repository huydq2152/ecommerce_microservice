using System.Text.Json;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Common.Constants;

namespace Infrastructure.Identity.Authorization;

public class ClaimRequirementFilter: IAuthorizationFilter
{
    private readonly FunctionCode _function;
    private readonly CommandCode _command;

    public ClaimRequirementFilter(FunctionCode function, CommandCode command)
    {
        _function = function;
        _command = command;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var permissionsClaim =
            context.HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals(SystemConstants.Claims.Permissions));
        if (permissionsClaim != null)
        {
            var permissions = JsonSerializer.Deserialize<List<string>>(permissionsClaim.Value);
            if(permissions == null || !permissions.Contains(PermissionHelper.GetPermission(_function, _command)))
            {
                context.Result = new ForbidResult();
            }
        }
        else
        {
            context.Result = new ForbidResult();
        }
    }
}