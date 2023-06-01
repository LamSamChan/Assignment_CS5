using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment_CS5.Database;
using Assignment_CS5.Models;
using Assignment_CS5.Services;
using Assignment_CS5.IServices;
using Assignment_CS5.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Assignment_CS5.Constants;
using Assignment_CS5.ViewModels;
using Newtonsoft.Json;

namespace Assignment_CS5.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly MyDbContext _context;
        private readonly ICustomerSvc _service;
        private readonly IUploadHelper _helper;
        private int isAuthenticate;
        public int IsAuthenticate
        {
            get
            {
                if (!String.IsNullOrEmpty(HttpContext.Session.GetString(SessionKey.Employee.UserName)))
                {
                    if(HttpContext.Session.GetString(SessionKey.Employee.Role) == "Admin")
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
        public CustomerController(MyDbContext context, ICustomerSvc service, IUploadHelper helper)
        {
            _context = context;
            _service = service;
            _helper = helper;
        }

        // GET: Menu
        [HttpGet]
        public IActionResult Index(string type, string searchString, int page)
        {

            if (IsAuthenticate == 1 || IsAuthenticate == 2)
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View(_service.GetAll(type, searchString, page));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }



        public IActionResult SignUp()
        {
                return View();
        }

        // POST: Menu/SignUp
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int check = _service.AddCustomer(customer);
                    if (check == -1)
                    {
                        TempData["Message"] = "Email already exists, try 1 different email.";
                        TempData["MessageType"] = "danger";
                        ViewBag.SHClass = "d-none";
                        ViewBag.bgblack = "bg-black";
                        return View("SignUp");
                    }
                    else if(check == -2){
                        TempData["Message"] = "Phone number already exists, try 1 different phone number.";
                        TempData["MessageType"] = "danger";
                        ViewBag.SHClass = "d-none";
                        ViewBag.bgblack = "bg-black";
                        return View("SignUp");
                    }
                    else {
                        TempData["SuccessMessage"] = "Đăng ký thành công!";

                        return RedirectToAction("Index", "Home");
                    }
                   
                }
                catch
                {
                    TempData["Message"] = "Adding an customer failed.";
                    TempData["MessageType"] = "danger";
                    ViewBag.SHClass = "d-none";
                    ViewBag.bgblack = "bg-black";
                    return View("SignUp");
                }
            }
            else
            {
                TempData["Message"] = "An error occurred";
                TempData["MessageType"] = "danger";
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View("SignUp");
            }

        }

        // GET: Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _service.UpdateCustomer(customer);
                return RedirectToAction("Index", "Customer");
            }
            else
            {
                TempData["Message"] = "An error occurred";
                TempData["MessageType"] = "danger";
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View("Edit", customer);
            }
        }

        public IActionResult Edit(int Id)
        {
            if (IsAuthenticate == 1 )
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                var emp = _service.GetById(Id);
                return View(emp);
            }
            else if(IsAuthenticate == 2 )
            {
                return RedirectToAction("Index", "Customer");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Info()
        {
            string cusEmail = HttpContext.Session.GetString(SessionKey.Customer.CusEmail);

            if (!String.IsNullOrEmpty(cusEmail))
            {
                return RedirectToAction("Index", "Home");
            }

            var cusContext = HttpContext.Session.GetString(SessionKey.Customer.CusContext);
            var cusId = JsonConvert.DeserializeObject<Customer>(cusContext).CustomerID;
            var customer = _service.GetById(cusId);
            ViewBag.SHClass = "d-none";
            ViewBag.bgblack = "bg-black";
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateForCus(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _service.UpdateCustomer(customer);
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Message"] = "An error occurred";
                TempData["MessageType"] = "danger";
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View("Info");
            }
        }

        public IActionResult Login()
		{
			string userName = HttpContext.Session.GetString(SessionKey.Customer.CusEmail);
			if (!String.IsNullOrEmpty(userName))
			{
				return RedirectToAction("Index", "Home");
			}
			else
			{
				return View();
			}
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult LoginAction(ViewLogin viewLogin)
		{
			if (ModelState.IsValid)
			{
				Customer cus = _service.Login(viewLogin);
                
                if (cus != null)
				{
					if (cus.Locked)
					{
						TempData["Message"] = "Your account is locked";
						TempData["MessageType"] = "danger";
						return View("Login");
					}
                    else
                    {
                        HttpContext.Session.SetString(SessionKey.Customer.CusEmail, cus.Email);
                        HttpContext.Session.SetString(SessionKey.Customer.CusFullName, cus.FullName);
                        HttpContext.Session.SetString(SessionKey.Customer.Role, "Customer");
                        HttpContext.Session.SetString(SessionKey.Customer.CusContext,
                            JsonConvert.SerializeObject(cus));

                        return RedirectToAction("Index", "Home");
                    }
				    
				}
				else
				{
					TempData["Message"] = "Wrong email or password, please check again";
					TempData["MessageType"] = "danger";
					return View("Login");
				}
			}
			TempData["Message"] = "Wrong email or password, please check again";
			TempData["MessageType"] = "danger";
			return View("Login");

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index", "Home");
		}
	}
}
