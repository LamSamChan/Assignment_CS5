using Assignment_CS5.Database;
using Assignment_CS5.Helpers;
using Assignment_CS5.IServices;
using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
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
        public int AddEmployee(Employee employee)
        {
            int status = 0;
            try
            {
                employee.Password = _helper.Encode(employee.Password);
                _context.Add(employee);
                _context.SaveChanges();
                status = employee.EmployeeID;
            }
            catch
            {
                status = 0;
            }
            return status;
        }

        public PaginationViewModel GetAll(string searchString, int page)
        {
            var list = _context.Employees.ToList();
            try
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    list = list.Where(p => p.FullName.ToLower().Contains(searchString.ToLower()) || 
                    p.PhoneNumber.ToLower().Contains(searchString.ToLower()) ||
                    p.Email.ToLower().Contains(searchString.ToLower())).ToList();
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


        public Employee GetById(int Id)
        {
            try
            {
               return _context.Employees.FirstOrDefault(m => m.EmployeeID == Id);
            }
            catch (System.Exception ex)
            {
                return new Employee();
            }
        }


        public int UpdateEmployee(Employee employee)
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

		public Employee Login(ViewLogin viewLogin)
        {
            var login = _context.Employees.Where(e => e.UserName.Equals(viewLogin.UserName) 
            && e.Password.Equals(_helper.Encode(viewLogin.Password))).FirstOrDefault();

            return login;
        }

	}
}
