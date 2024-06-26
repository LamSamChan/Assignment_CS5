﻿using Assignment_CS5.Constants;
using Assignment_CS5.IServices;
using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Assignment_CS5.Controllers
{
	public class AdminController : Controller
	{
		private readonly IEmployeeSvc _employeeSvc;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public AdminController(IEmployeeSvc employeeSvc, IWebHostEnvironment webHostEnvironment)
		{
			_employeeSvc = employeeSvc;
			_webHostEnvironment = webHostEnvironment;
		}

		[Route("Admin")]
		public IActionResult Login()
		{
			string userName = HttpContext.Session.GetString(SessionKey.Employee.UserName);
            string cus = HttpContext.Session.GetString(SessionKey.Customer.CusEmail);

            if (!String.IsNullOrEmpty(userName) || !String.IsNullOrEmpty(cus))
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
				Employee emp = _employeeSvc.Login(viewLogin);
				if (emp.Locked)
				{
                    TempData["Message"] = "Your account is locked";
                    TempData["MessageType"] = "danger";
                    return View("Login");
                }else
				if (emp != null)
				{
					HttpContext.Session.SetString(SessionKey.Employee.UserName, emp.UserName);
					HttpContext.Session.SetString(SessionKey.Employee.FullName, emp.FullName);

                    if (emp.Position.ToString() == "Manager")
					{
                        HttpContext.Session.SetString(SessionKey.Employee.Role, "Admin");
					}
					else
					{
                        HttpContext.Session.SetString(SessionKey.Employee.Role, "Employee");
                    }
					
					HttpContext.Session.SetString(SessionKey.Employee.EmployeeContext,
						JsonConvert.SerializeObject(emp));

					return RedirectToAction("Index", "Home");
				}
				else
				{
					TempData["Message"] = "Wrong username or password, please check again";
					TempData["MessageType"] = "danger";
					return View("Login");
				}
			}
			TempData["Message"] = "Wrong username or password, please check again";
			TempData["MessageType"] = "danger";
			return View("Login");

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Logout()
		{
			HttpContext.Session.SetString(SessionKey.Employee.UserName, "");
			HttpContext.Session.SetString(SessionKey.Employee.FullName, "");
			HttpContext.Session.SetString(SessionKey.Employee.Role, "");
			HttpContext.Session.SetString(SessionKey.Employee.EmployeeContext,
				JsonConvert.SerializeObject(""));
			return RedirectToAction("Index", "Home");
		}
	}
}
