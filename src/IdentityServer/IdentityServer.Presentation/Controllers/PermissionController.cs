using IdentityServer.Infrastructure.Repositories;
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
}