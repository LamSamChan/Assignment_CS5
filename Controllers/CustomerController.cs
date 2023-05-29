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

namespace Assignment_CS5.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly MyDbContext _context;
        private readonly ICustomerSvc _service;
        private readonly IUploadHelper _helper;
        private bool isAdmin;
        public bool IsAdmin
        {
            get
            {
                if (String.IsNullOrEmpty(HttpContext.Session.GetString(SessionKey.Employee.UserName))
                    && HttpContext.Session.GetString(SessionKey.Employee.Role) != "Admin")
                {
                    isAdmin = false;
                }
                else
                {
                    isAdmin = true;
                }
                return isAdmin;
            }
            set { this.isAdmin = value; }
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

            if (IsAdmin)
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



        public IActionResult Create()
        {
                return View();
        }

        // POST: Menu/Create
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
                        return View("Create");
                    }
                    else if(check == -1){
                        TempData["Message"] = "Phone number already exists, try 1 different phone number.";
                        TempData["MessageType"] = "danger";
                        ViewBag.SHClass = "d-none";
                        ViewBag.bgblack = "bg-black";
                        return View("Create");
                    }
                    else {
                        return RedirectToAction("Index");
                    }
                   
                }
                catch
                {
                    TempData["Message"] = "Adding an customer failed.";
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

        // GET: Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _service.UpdateCustomer(customer);
                return RedirectToAction("Index");
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
            if (IsAdmin)
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                var emp = _service.GetById(Id);
                return View(emp);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
