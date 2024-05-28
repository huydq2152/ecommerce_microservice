using IdentityServer.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Presentation.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IRepositoryManager _repositoryManager;

    public PermissionController(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }
}