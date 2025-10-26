using Microsoft.AspNetCore.Mvc;

namespace Web_WhaleBooking.Controllers;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        return View();
    }
}
