using Microsoft.AspNetCore.Mvc;

namespace Saga.Orchestrator.Controllers;

public class HomeController: ControllerBase
{
    public IActionResult Index()
    {
        return Redirect($"~/swagger");
    }
}