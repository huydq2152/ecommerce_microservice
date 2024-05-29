using System.ComponentModel.DataAnnotations;
using System.Net;
using IdentityServer.Infrastructure.Repositories;
using IdentityServer.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Presentation.Controllers;

[ApiController]
[Route("api/roles/{roleId}/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IRepositoryManager _repositoryManager;

    public PermissionController(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetPermissions(string roleId)
    {
        var result = await _repositoryManager.Permission.GetPermissionsByRole(roleId);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PermissionViewModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreatePermission(string roleId, [FromBody] PermissionAddModel model)
    {
        var result = await _repositoryManager.Permission.CreatePermission(roleId, model);
        return result != null ? Ok(result) : NoContent();
    }

    [HttpDelete("function/{function}/command/{command}")]
    [ProducesResponseType(typeof(PermissionViewModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeletePermission(string roleId, [Required] string function,
        [Required] string command)
    {
        await _repositoryManager.Permission.DeletePermission(roleId, function, command);
        return NoContent();
    }

    [HttpPost("update-permissions")]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdatePermissions(string roleId,
        [FromBody] IEnumerable<PermissionAddModel> permissions)
    {
        await _repositoryManager.Permission.UpdatePermissionsByRoleId(roleId, permissions);
        return NoContent();
    }
}