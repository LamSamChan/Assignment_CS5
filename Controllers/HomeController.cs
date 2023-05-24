using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Assignment_CS5.Models;
using Assignment_CS5.Database;
using Assignment_CS5.Services;
using Assignment_CS5.IServices;

namespace Assignment_CS5.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MyDbContext _context;
    private readonly IMenuSvc _service;
    public HomeController(ILogger<HomeController> logger, MyDbContext context, IMenuSvc service)
    {
        _logger = logger;
        _context = context;
        _service = service;
    }

    public IActionResult Index()
    {
        ViewBag.SHClass = "d-block";
        return View();
    }
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
