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
    public class MenuController : BaseController
    {
        private readonly MyDbContext _context;
        private readonly IMenuSvc _service;
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
        public MenuController(MyDbContext context, IMenuSvc service, IUploadHelper helper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _service = service;
            _helper = helper;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Menu
        [HttpGet]
        public IActionResult Index(string searchString,int page)
        {
            if (IsAuthenticate == 1 || IsAuthenticate == 2)
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
            if (IsAuthenticate == 1)
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                return View();
            }
            else if (IsAuthenticate == 2)
            {
                return RedirectToAction("Index", "Menu");
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
        public IActionResult Add(Menu menu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (menu.ImageFile.ContentType.StartsWith("image/"))
                    {
                        if (menu.ImageFile != null)
                        {
                            if (menu.ImageFile.Length > 0)
                            {
                                string rootPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                                _helper.UploadImage(menu.ImageFile, rootPath, "products");
                                menu.Image = menu.ImageFile.FileName;
                            }
                            _service.AddProduct(menu);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["Message"] = "Adding food failed, upload a photo file of the product";
                            TempData["MessageType"] = "danger";
                            ViewBag.SHClass = "d-none";
                            ViewBag.bgblack = "bg-black";
                            return View("Create");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Adding food failed, upload a photo file of the product";
                        TempData["MessageType"] = "danger";
                        ViewBag.SHClass = "d-none";
                        ViewBag.bgblack = "bg-black";
                        return View("Create");
                    }
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
                return View("Create",menu);
            }

        }

        // GET: Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Menu menu)
        {
            Menu existingProduct = _service.GetById(menu.ProductId);
            if (ModelState.IsValid)
            {
                if (menu.ImageFile != null)
                {
                    if (menu.ImageFile.ContentType.StartsWith("image/"))
                    {
                        if (menu.ImageFile.Length > 0)
                        {
                            string rootPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                            _helper.UploadImage(menu.ImageFile, rootPath, "products");
                            menu.Image = menu.ImageFile.FileName;
                        }
                    }
                    else
                    {
                        ViewBag.SHClass = "d-none";
                        ViewBag.bgblack = "bg-black";
                        TempData["Message"] = "Please upload an image file of the product";
                        TempData["MessageType"] = "danger";
                        return View("Edit", existingProduct);
                    }
                }
                else
                {
                    menu.Image = existingProduct.Image;
                }
                _service.UpdateProduct(menu);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                TempData["Message"] = "An error occurred";
                TempData["MessageType"] = "danger";
                return View("Edit", existingProduct);
            }
        }

        public IActionResult Edit(int Id)
        {

            if (IsAuthenticate == 1)
            {
                ViewBag.SHClass = "d-none";
                ViewBag.bgblack = "bg-black";
                var productInMenu = _service.GetById(Id);
                return View(productInMenu);
            }
            else if (IsAuthenticate == 2)
            {
                return RedirectToAction("Index", "Menu");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
