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
    public class EmployeeController : BaseController
    {
        private readonly MyDbContext _context;
        private readonly IEmployeeSvc _service;
        private readonly IUploadHelper _helper;
        private readonly IWebHostEnvironment _webHostEnvironment;
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
        public EmployeeController(MyDbContext context, IEmployeeSvc service, IUploadHelper helper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _service = service;
            _helper = helper;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Menu
        [HttpGet]
        public IActionResult Index(string searchString, int page)
        {
            
            if (IsAdmin)
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View(_service.GetAll(searchString, page));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }



        // GET: Menu/Create
        public IActionResult Create()
        {
            if (IsAdmin)
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (employee.ImageFile.ContentType.StartsWith("image/"))
                    {
                        if (employee.ImageFile != null)
                        {
                            if (employee.ImageFile.Length > 0)
                            {
                                string rootPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                                _helper.UploadImage(employee.ImageFile, rootPath, "avatars");
                                employee.Image = employee.ImageFile.FileName;
                            }
                        }
                        _service.AddEmployee(employee);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Message"] = "Adding an employee failed, upload an employee's image file";
                        TempData["MessageType"] = "danger";
                        ViewBag.SHClass = "d-none";
                        ViewBag.bgblack = "bg-black";
                        return View("Create");
                    }
                }
                catch
                {
                    TempData["Message"] = "Adding an employee failed.";
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
        public IActionResult Update(Employee employee)
        {
            Employee existingEmp = _service.GetById(employee.EmployeeID);
            if (ModelState.IsValid)
            {
                if (employee.ImageFile != null)
                {
                    if (employee.ImageFile.ContentType.StartsWith("image/"))
                    {
                        if (employee.ImageFile.Length > 0)
                        {
                            string rootPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                            _helper.UploadImage(employee.ImageFile, rootPath, "avatars");
                            employee.Image = employee.ImageFile.FileName;
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Please upload a photo file of the employee";
                        TempData["MessageType"] = "danger";
                        return View("Edit", existingEmp);
                    }
                }
                else
                {
                    employee.Image = existingEmp.Image;
                }
                _service.UpdateEmployee(employee);
                return RedirectToAction("Index");
        }
            else
            {
                TempData["Message"] = "An error occurred";
                TempData["MessageType"] = "danger";
                return View("Edit", existingEmp);
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
