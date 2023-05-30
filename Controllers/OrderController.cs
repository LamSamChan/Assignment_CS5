﻿using Assignment_CS5.Constants;
using Assignment_CS5.Database;
using Assignment_CS5.IServices;
using Assignment_CS5.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_CS5.Controllers
{
    public class OrderController : BaseController
    {
        private readonly MyDbContext _context;
        private readonly IOrderSvc _service;
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
        public OrderController(MyDbContext context, IOrderSvc service)
        {
            this._context = context;
            this._service = service;
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

        public IActionResult Details(int Id)
        {
            if (IsAuthenticate == 1 || IsAuthenticate == 2)
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

    }
}
