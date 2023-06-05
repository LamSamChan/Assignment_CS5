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
    public class EmployeeController : BaseController
    {
        private readonly MyDbContext _context;
        private readonly IEmployeeSvc _service;
        private readonly IUploadHelper _helper;
        private readonly IWebHostEnvironment _webHostEnvironment;
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
        public EmployeeController(MyDbContext context, IEmployeeSvc service, IUploadHelper helper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _service = service;
            _helper = helper;
            _webHostEnvironment = webHostEnvironment;
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

		public IActionResult ChangePassword()
		{
			string empEmail = HttpContext.Session.GetString(SessionKey.Employee.UserName);

			if (String.IsNullOrEmpty(empEmail))
			{
				return RedirectToAction("Index", "Home");
			}
			else
			{
				return View();
			}


		}
        public IActionResult ChangePw(ChangePassword changePassword)
        {
            if (ModelState.IsValid)
            {
                var empContext = HttpContext.Session.GetString(SessionKey.Employee.EmployeeContext);
                var empId = JsonConvert.DeserializeObject<Employee>(empContext).EmployeeID;
                int result = _service.ChangePassword(empId, changePassword);
                if (result == 0)
                {
                    TempData["Message"] = "Old password is not correct";
                    TempData["MessageType"] = "danger";
                    ViewBag.SHClass = "d-none";
                    ViewBag.bgblack = "bg-black";
                    return View("ChangePassword");
                }
                else
                {
                    ViewBag.SHClass = "d-none";
                    ViewBag.bgblack = "bg-black";
                    TempData["SuccessMessage"] = "Change password successfully !";

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Message"] = "An error occurred";
                TempData["MessageType"] = "danger";
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View("ChangePassword");
            }

        }
			// GET: Menu/Create
			public IActionResult Create()
        {

            if (IsAuthenticate == 1)
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View();
            }
            else if(IsAuthenticate == 2)
            {
                return RedirectToAction("Index", "Employee");
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
                        int check = _service.AddEmployee(employee);
                        if (check == -1)
                        {
                            TempData["Message"] = "Username already exists, try 1 different name";
                            TempData["MessageType"] = "danger";
                            ViewBag.SHClass = "d-none";
                            ViewBag.bgblack = "bg-black";
                            return View("Create");
                        }
                        else if(check == -2)
                        {
                            TempData["Message"] = "Email already exists, try 1 different email";
                            TempData["MessageType"] = "danger";
                            ViewBag.SHClass = "d-none";
                            ViewBag.bgblack = "bg-black";
                            return View("Create");
                        }
                        else if (check == -3)
                        {
                            TempData["Message"] = "Phone number already exists, try 1 different phone number";
                            TempData["MessageType"] = "danger";
                            ViewBag.SHClass = "d-none";
                            ViewBag.bgblack = "bg-black";
                            return View("Create");
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                       
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
                        ViewBag.SHClass = "d-none";
                        ViewBag.bgblack = "bg-black";
                        return View("Edit");
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
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                TempData["Message"] = "An error occurred";
                TempData["MessageType"] = "danger";
                return View("Edit");
            }
        }

        public IActionResult Edit(int Id)
        {
            if (IsAuthenticate == 1)
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                var emp = _service.GetById(Id);
                return View(emp);
            }
            else if (IsAuthenticate == 2)
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

		public IActionResult Info()
		{
			string emp = HttpContext.Session.GetString(SessionKey.Employee.UserName);

			if (String.IsNullOrEmpty(emp))
			{
				return RedirectToAction("Index", "Home");
			}

			var empContext = HttpContext.Session.GetString(SessionKey.Employee.EmployeeContext);
			var empId = JsonConvert.DeserializeObject<Employee>(empContext).EmployeeID;
			var employee = _service.GetById(empId);
			ViewBag.SHClass = "d-none";
			ViewBag.bgblack = "bg-black";
			return View(employee);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateForEmp(Employee employee)
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
                        TempData["Message"] = "Please upload your photo file";
                        TempData["MessageType"] = "danger";
                        ViewBag.SHClass = "d-none";
                        ViewBag.bgblack = "bg-black";
                        return View("Info");
                    }
                }
                else
                {
                    employee.Image = existingEmp.Image;
                }
                _service.UpdateEmployee(employee);
                TempData["SuccessMessage"] = "Updated information successfully!";
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

	}
}
