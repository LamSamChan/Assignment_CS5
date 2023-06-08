using Assignment_CS5.Database;
using Assignment_CS5.Helpers;
using Assignment_CS5.IServices;
using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using static Assignment_CS5.Constants.SessionKey;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Assignment_CS5.Services
{
    public class EmployeeSvc : IEmployeeSvc
    {
        private readonly MyDbContext _context;
        private readonly IEncodeHelper _helper;

        public EmployeeSvc(MyDbContext context,IEncodeHelper encodeHelper)
        {
            this._context = context;
            this._helper = encodeHelper;
        }
        public int AddEmployee(Models.Employee employee)
        {
            int status = 0;
            try
            {
                if(IsFieldExist(employee) != 0)
                {
                    return IsFieldExist(employee);
                }
                else
                {
                    employee.Password = _helper.Encode(employee.Password);
                    _context.Add(employee);
                    _context.SaveChanges();
                    status = employee.EmployeeID;
                }
            }
            catch
            {
                status = 0;
            }
            return status;
        }

        public PaginationViewModel GetAll(string type,string searchString, int page)
        {
            var list = _context.Employees.ToList();
            try
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    if (type == "ID")
                    {
                        list = list.Where(c => c.EmployeeID.ToString().Contains(searchString.ToLower())).ToList();
                    }
                    else if (type == "FN")
                    {
                        list = list.Where(c => c.FullName.ToLower().Contains(searchString.ToLower())).ToList();
                    }
                    else if (type == "PN")
                    {
                        list = list.Where(c => c.PhoneNumber.Contains(searchString.ToLower())).ToList();
                    }
                    else
                    {
                        list = list.Where(c => c.Email.Contains(searchString.ToLower())).ToList();
                    }
                }
                int pageSize = 5;
                var pagedEmployees = list.Skip((page - 1) * pageSize).Take(pageSize); // Lấy các sản phẩm cho trang hiện tại

                var viewModel = new PaginationViewModel
                {
                    Employees = pagedEmployees,
                    PaginationInfo = new PaginationInfo
                    {
                        SearchKeyword = searchString,
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = list.Count()
                    }
                };
                return viewModel;
                
            }
            catch (System.Exception ex)
            {
                return new PaginationViewModel();
            }
        }


        public Models.Employee GetById(int Id)
        {
            try
            {
               return _context.Employees.FirstOrDefault(m => m.EmployeeID == Id);
            }
            catch (System.Exception ex)
            {
                return new Models.Employee();
            }
        }


        public int UpdateEmployee(Models.Employee employee)
        {
            int status = 0;
            try
            {
                // Lấy đối tượng từ database dựa vào id
                var existingEmp = _context.Employees.FirstOrDefault(m => m.EmployeeID == employee.EmployeeID);

                // Kiểm tra xem đối tượng có tồn tại trong database không
                if (existingEmp == null)
                {
                    return 0;
                }

                // Cập nhật thông tin của đối tượng 
                existingEmp.EmployeeID = employee.EmployeeID;
                existingEmp.UserName = employee.UserName;
                existingEmp.FullName = employee.FullName;
                existingEmp.Email = employee.Email;
                existingEmp.Position = employee.Position;
                existingEmp.Gender = employee.Gender;
                existingEmp.DateOfBirth = employee.DateOfBirth;
                existingEmp.PhoneNumber = employee.PhoneNumber;
                existingEmp.Locked = employee.Locked;
                existingEmp.Image = employee.Image;
                existingEmp.Note = employee.Note;
                if (employee.Password == null) {
                    existingEmp.Password = existingEmp.Password;
                }

                // Lưu thay đổi vào database
                _context.SaveChanges();
                status = employee.EmployeeID;
            }
            catch (System.Exception ex)
            {
                status = 0;
            }
            return status;
        }

		public Models.Employee Login(ViewLogin viewLogin)
        {
            var login = _context.Employees.Where(e => e.UserName.Equals(viewLogin.UserName) 
            && e.Password.Equals(_helper.Encode(viewLogin.Password))).FirstOrDefault();

            return login;
        }

        public int IsFieldExist(Models.Employee employee)
        {
            string phoneNumber = null;

            if (employee.PhoneNumber.StartsWith("+84"))
            {
                phoneNumber = employee.PhoneNumber.Substring(3);
            }
            else if (employee.PhoneNumber.StartsWith("0"))
            {
                phoneNumber = employee.PhoneNumber.Substring(1);
            }

            foreach (var emp in _context.Employees.ToList())
            {
                string existPhoneNumber = null;
                if (emp.PhoneNumber.StartsWith("+84"))
                {
                    existPhoneNumber = emp.PhoneNumber.Substring(3);
                }
                else if (emp.PhoneNumber.StartsWith("0"))
                {
                    existPhoneNumber = emp.PhoneNumber.Substring(1);
                }

                if (employee.UserName == emp.UserName)
                {
                    return -1;
                }
                else if(employee.Email == emp.Email)
                {
                    return -2;
                }
                else if (phoneNumber == existPhoneNumber)
                {
                    return -3;
                }
            }

            return 0;
        }

		public int ChangePassword(int empId, ChangePassword changePassword)
		{
			var emp = GetById(empId);
			if (emp.Password != _helper.Encode(changePassword.Password))
			{
				return 0;
			}
			else
			{
				emp.Password = _helper.Encode(changePassword.NewPassword);
                UpdateEmployee(emp);
				return 1;

			}
		}
	}
}
