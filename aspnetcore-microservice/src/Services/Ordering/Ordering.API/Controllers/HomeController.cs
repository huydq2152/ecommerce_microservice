using Microsoft.AspNetCore.Mvc;

namespace Ordering.API.Controllers;

public class HomeController : ControllerBase
{
    //GET
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}