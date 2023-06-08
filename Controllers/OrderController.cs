using Assignment_CS5.Constants;
using Assignment_CS5.Database;
using Assignment_CS5.IServices;
using Assignment_CS5.Models;
using Assignment_CS5.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_CS5.Controllers
{
    public class OrderController : BaseController
    {
        private readonly MyDbContext _context;
        private readonly IOrderSvc _service;
        private readonly IPayPalService _ppService;
        private readonly ICustomerSvc _cusService;
        private int isAuthenticate;
        public int IsAuthenticate
        {
            get
            {
                if (!String.IsNullOrEmpty(HttpContext.Session.GetString(SessionKey.Employee.UserName)))
                {
                    if (HttpContext.Session.GetString(SessionKey.Employee.Role) == "Admin")
                    {
                        isAuthenticate = 1; //Admin
                    }
                    else
                    {
                        isAuthenticate = 2; //Emp
                    }
                }
                else
                {
                    isAuthenticate = 3;//Cus
                }
                return isAuthenticate;
            }
            set { this.isAuthenticate = value; }
        }
        public OrderController(MyDbContext context, IOrderSvc service, ICustomerSvc cusService, IPayPalService ppService)
        {
            this._context = context;
            this._service = service;
            this._cusService = cusService;
            this._ppService = ppService;

        }
        public IActionResult Index(string type, string searchString, DateTime searchDate, int page)
        {
            if (IsAuthenticate == 1 || IsAuthenticate == 2)
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View(_service.GetAll(type, searchString, searchDate, page));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult History(string searchString, DateTime searchDate, int page)
        {
            var cusEmail = HttpContext.Session.GetString(SessionKey.Customer.CusEmail);
            int cusId=0;
            if (!String.IsNullOrEmpty(cusEmail))
            {
                cusId = _context.Customer.FirstOrDefault(c => c.Email == cusEmail).CustomerID;
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.SHClass = "d-none";
            ViewBag.bgblack = "bg-black";
            return View(_service.GetAllForCus(cusId,searchString, searchDate, page));
            
        }
        public IActionResult Create()
        {
                return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Order order)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _service.AddOrder(order);
                    return RedirectToAction("Index");
                }
                catch
                {
                    TempData["Message"] = "Adding food failed.";
                    TempData["MessageType"] = "danger";
                    ViewBag.SHClass = "d-none";
                    ViewBag.bgblack = "bg-black";
                    return View("Create");
                }
            }
            else
            {
                TempData["Message"] = "An error occurred";
                TempData["MessageType"] = "danger";
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View("Create");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Order order)
        {
            if (ModelState.IsValid)
            {
                var cusId = order.CustomerId;
                var cus = _cusService.GetById(cusId);
                if (order.PointAdded == true)
                {
                    if (order.Status.ToString() == "Cancelled" || order.Delete == true)
                    {
                        int point = Convert.ToInt32(cus.Point - (order.Total / 1000));
                        cus.Point = point;
                        order.PointAdded = false;
                    }
                    
                }else
                {
                    if (order.Status.ToString() != "Cancelled" && order.Delete == false)
                    {
                        int point = Convert.ToInt32(cus.Point + (order.Total / 1000));
                        cus.Point = point;
                        order.PointAdded = true;
                    }     
                }
                _cusService.UpdateCustomer(cus);
                _service.UpdateOrder(order);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = "An error occurred";
                TempData["MessageType"] = "danger";
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View("Edit", order);
            }
        }

        public IActionResult Edit(int Id)
        {
            if (IsAuthenticate == 1 || IsAuthenticate ==2)
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                var order = _service.GetById(Id);
                ViewBag.CustomerName = order.Customer.FullName;
                return View(order);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Details(string Id)
        {
            var cusEmail = HttpContext.Session.GetString(SessionKey.Customer.CusEmail);

            if (IsAuthenticate == 1 || IsAuthenticate == 2 || !String.IsNullOrEmpty(cusEmail))
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View(_service.GetOrderDetails(Id));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult PaymentResponse(string searchString, int page)
        {
            if (IsAuthenticate == 1 || IsAuthenticate == 2)
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View(_ppService.GetAll(searchString, page));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
