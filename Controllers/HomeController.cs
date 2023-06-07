using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Assignment_CS5.Models;
using Assignment_CS5.Database;
using Assignment_CS5.Services;
using Assignment_CS5.IServices;
using Assignment_CS5.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using Assignment_CS5.Constants;
using NuGet.Protocol;

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
    private readonly IPayPalService _payPalService;
    public HomeController(ILogger<HomeController> logger, MyDbContext context, IMenuSvc menuSvc, IWebHostEnvironment webHostEnvironment,
        ICustomerSvc customerSvc, IOrderSvc orderSvc, IOrderDetailSvc orderDetailSvc, IPayPalService payPalService)
    {
        _logger = logger;
        _context = context;
        _menuSvc = menuSvc;
        _webHostEnvironment = webHostEnvironment;
        _customerSvc = customerSvc; 
        _orderSvc = orderSvc;
        _orderDetailSvc = orderDetailSvc;
        _payPalService = payPalService;

    }
    public async Task<IActionResult> CreatePaymentUrl()
    {
        var cart = HttpContext.Session.GetString("cart");
        double total = Sum();
        List<ViewCart> model = JsonConvert.DeserializeObject<List<ViewCart>>(cart);
        for (int i = 0; i < model.Count; i++)
        {
            if (model[i].Menu.Status != _menuSvc.GetById(model[i].Menu.ProductId).Status)
            {
                var failMessage = $"{model[i].Menu.Name} is out of stock!";
                return Json(new { message = failMessage });
            }
        }
        var url = await _payPalService.CreatePaymentUrl(model,total);

        return Content(url);
    }

    public IActionResult PaymentCallback()
    {
        var response = _payPalService.PaymentExecute(Request.Query);
        var payRes = JsonConvert.DeserializeObject<PaymentResponse>(response.ToJson());
        string cusEmail = HttpContext.Session.GetString(SessionKey.Customer.CusFullName);
        var cart = HttpContext.Session.GetString("cart");
        var cusContext = HttpContext.Session.GetString(SessionKey.Customer.CusContext);
        var cusId = JsonConvert.DeserializeObject<Customer>(cusContext).CustomerID;
        List<ViewCart> dataCart = JsonConvert.DeserializeObject<List<ViewCart>>(cart);

        if (cusEmail == null || cusEmail == "")  // đã có session
        {
            return BadRequest();
        }
        if (payRes.Success)
        {
            

            double total = Sum();


            var order = new Order()
            {
                OrderId = payRes.OrderId,
                Status = OrderStatus.Received,
                CustomerId = cusId,
                Total = total,
                Method = payRes.PaymentMethod,
                OrderDate = DateTime.Now,
                Note = "",
            };

            _orderSvc.AddOrder(order);
            string orderId = order.OrderId;

            for (int i = 0; i < dataCart.Count; i++)
            {
                OrderDetails details = new OrderDetails()
                {
                    OrderId = orderId,
                    ProductId = dataCart[i].Menu.ProductId,
                    Quantity = dataCart[i].Quantity,
                    Total = dataCart[i].Menu.Price * dataCart[i].Quantity,
                    Note = dataCart[i].Note,
                };
                //donhang.DonhangChitiets.Add(chitiet);
                _orderDetailSvc.AddOrderDetail(details);
            }
            _payPalService.AddPaymentRespone(payRes);
            HttpContext.Session.Remove("cart");

            return RedirectToAction("History", "Order");
        }
        else return BadRequest();
    }
    public IActionResult Index()
    {
        ViewBag.SHClass = "d-block";
        return View(_menuSvc.GetAllMenu());
    }
    public IActionResult ViewCart()
    {
        List<ViewCart> dataCart = new List<ViewCart>();
        var cart = HttpContext.Session.GetString("cart");
        if(!String.IsNullOrEmpty(cart))
        {
            dataCart = JsonConvert.DeserializeObject<List<ViewCart>>(cart);
        }
        ViewBag.SHClass = "d-none";
        ViewBag.bgblack = "bg-black";
        return View(dataCart);
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
                    Quantity = 1,
                    Note =""
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

    [HttpPost]
    public IActionResult UpdateCart(int id, int quantity, string note)
    {
        var cart = HttpContext.Session.GetString("cart");
        double total = 0;
        if (!String.IsNullOrEmpty(cart))
        {
            List<ViewCart> dataCart = JsonConvert.DeserializeObject<List<ViewCart>>(cart);
            for (int i = 0; i < dataCart.Count; i++)
            {
                if (dataCart[i].Menu.ProductId == id)
                {
                    dataCart[i].Note = note;
                    dataCart[i].Quantity = quantity;
                    break;
                }
            }
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));

            total = Sum();
            return Ok(total);
        }
        return BadRequest();
    }

    public IActionResult DeleteCart(int id)
    {
        double total = 0;
        var cart = HttpContext.Session.GetString("cart");
        if (cart != null)
        {
            List<ViewCart> dataCart = JsonConvert.DeserializeObject<List<ViewCart>>(cart);

            for (int i = 0; i < dataCart.Count; i++)
            {
                if (dataCart[i].Menu.ProductId == id)
                {
                    dataCart.RemoveAt(i);
                }
            }
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
            total = Sum();
            return Ok(total);
        }
        
        return BadRequest();
    }

    public IActionResult OrderCart()
    {
        string cusEmail = HttpContext.Session.GetString(SessionKey.Customer.CusFullName);
        if (cusEmail == null || cusEmail == "")  // đã có session
        {
            return BadRequest();
        }
        var cart = HttpContext.Session.GetString("cart");
        if (cart != "[]" || !String.IsNullOrEmpty(cart))
        {
            
            var cusContext = HttpContext.Session.GetString(SessionKey.Customer.CusContext);
            var cusId = JsonConvert.DeserializeObject<Customer>(cusContext).CustomerID;

            List<ViewCart> dataCart = JsonConvert.DeserializeObject<List<ViewCart>>(cart);
            for (int i = 0; i < dataCart.Count; i++)
            {
                if (dataCart[i].Menu.Status != _menuSvc.GetById(dataCart[i].Menu.ProductId).Status)
                {
                    var failMessage = $"{dataCart[i].Menu.Name} is out of stock!";
                    return Json(new { success = false, message = failMessage });


                }
            }
            double total = Sum();


            var order = new Order()
            {
                OrderId = DateTime.Now.Ticks.ToString(),
                Status = OrderStatus.Received,
                CustomerId = cusId,
                Total = total,
                Method = "Cash",
                OrderDate = DateTime.Now,
                Note = "",
            };

            _orderSvc.AddOrder(order);
            string orderId = order.OrderId;
           
            
            for (int i = 0; i < dataCart.Count; i++)
            {
                OrderDetails details = new OrderDetails()
                {
                    OrderId = orderId,
                    ProductId = dataCart[i].Menu.ProductId,
                    Quantity = dataCart[i].Quantity,
                    Total = dataCart[i].Menu.Price * dataCart[i].Quantity,
                    Note = dataCart[i].Note,
                };
                //donhang.DonhangChitiets.Add(chitiet);
                _orderDetailSvc.AddOrderDetail(details);
            }

            

            HttpContext.Session.Remove("cart");

            return Json(new { success = true });
        }
        return BadRequest();
    }


    [NonAction]
    private double Sum()
    {
        double total = 0;
        var cart = HttpContext.Session.GetString("cart");
        if (cart != null)
        {
            List<ViewCart> dataCart = JsonConvert.DeserializeObject<List<ViewCart>>(cart);
            for (int i = 0; i < dataCart.Count; i++)
            {
                total += (dataCart[i].Menu.Price * dataCart[i].Quantity);
            }
        }
        return total;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}