using Microsoft.AspNetCore.Mvc;

namespace Web_WhaleBooking.Controllers;

public class MapController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
