using Microsoft.AspNetCore.Mvc;

namespace Web_WhaleBooking.Controllers;

public class OwnerController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        ViewData["Id"] = id;
        return View();
    }

    [HttpGet]
    public IActionResult Rooms(int coSoId)
    {
        ViewData["CoSoId"] = coSoId;
        return View();
    }

    [HttpGet]
    public IActionResult CreateRoom(int coSoId)
    {
        ViewData["CoSoId"] = coSoId;
        return View();
    }

    [HttpGet]
    public IActionResult EditRoom(int coSoId, int id)
    {
        ViewData["CoSoId"] = coSoId;
        ViewData["Id"] = id;
        return View();
    }
}
