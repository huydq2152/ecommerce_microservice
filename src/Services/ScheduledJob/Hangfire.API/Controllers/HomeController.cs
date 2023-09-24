using Microsoft.AspNetCore.Mvc;

namespace Hangfire.API.Controllers;

public class HomeController: ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}