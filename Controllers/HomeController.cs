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
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IMenuSvc _menuSvc;
    private readonly ICustomerSvc _customerSvc;
    private readonly IOrderSvc _orderSvc;
    private readonly IOrderDetailSvc _orderDetailSvc;

    public HomeController(ILogger<HomeController> logger, MyDbContext context, IMenuSvc menuSvc, IWebHostEnvironment webHostEnvironment,
        ICustomerSvc customerSvc, IOrderSvc orderSvc, IOrderDetailSvc orderDetailSvc)
    {
        _logger = logger;
        _context = context;
        _menuSvc = menuSvc;
        _webHostEnvironment = webHostEnvironment;
        _customerSvc = customerSvc; 
        _orderSvc = orderSvc;
        _orderDetailSvc = orderDetailSvc;
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
