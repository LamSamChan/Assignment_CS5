using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Assignment_CS5.Models;
using Assignment_CS5.Database;
using Assignment_CS5.Services;
using Assignment_CS5.IServices;
using Assignment_CS5.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;

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
        return View(_menuSvc.GetAllMenu());
    }
    public IActionResult AddCart(int id)
    {
        ViewBag.SHClass = "d-block";
        var cart = HttpContext.Session.GetString("cart");
        if (cart == null) {
            var item = _menuSvc.GetById(id);
            List<ViewCart> listCart = new List<ViewCart>()
            {
                new ViewCart
                {
                    Menu = item,
                    Quantity = 1
                }
            };
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));
        }
        else
        {
            List<ViewCart> dataCart = JsonConvert.DeserializeObject<List<ViewCart>>(cart);
            bool check = true;
            for (int i = 0; i < dataCart.Count; i++)
            {
                if (dataCart[i].Menu.ProductId == id)
                {
                    dataCart[i].Quantity++;
                    check = false;
                }
            }
            if (check)
            {
                var item = _menuSvc.GetById(id);
                dataCart.Add(new ViewCart()
                {
                    Menu = item,
                    Quantity = 1
                });

            }
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
        }
        return Ok();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
