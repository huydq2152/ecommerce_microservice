using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}