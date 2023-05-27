using Assignment_CS5.Database;
using Assignment_CS5.Helpers;
using Assignment_CS5.IServices;
using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using static Assignment_CS5.Constants.SessionKey;

namespace Assignment_CS5.Services
{
    public class CustomerSvc : ICustomerSvc
    {
        private readonly MyDbContext _context;
        private readonly IEncodeHelper _EncodeHelper;
        public CustomerSvc(MyDbContext context, IEncodeHelper encodeHelper)
        {
            this._context = context;
            _EncodeHelper = encodeHelper;
        }
        public int AddCustomer(Models.Customer customer)
        {
            int status = 0;
            try
            {
                customer.Password = _EncodeHelper.Encode(customer.Password);
                _context.Add(customer);
                _context.SaveChanges();
                status = customer.CustomerID;
            }
            catch
            {
                status = 0;
            }
            return status;
        }

        public PaginationViewModel GetAll( string searchString, int page)
        {
            var list = _context.Customer.ToList();
            try
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                        list = list.Where(p => p.PhoneNumber.Contains(searchString.ToLower())||
                        p.FullName.Contains(searchString.ToLower()) ||
                        p.CustomerID.ToString().Contains(searchString.ToLower())).ToList();
                }
                int pageSize = 5;
                var pagedCustomer = list.Skip((page - 1) * pageSize).Take(pageSize); // Lấy các sản phẩm cho trang hiện tại

                var viewModel = new PaginationViewModel
                {
                    Customers = pagedCustomer,
                    PaginationInfo = new PaginationInfo
                    {
                        SearchKeyword = searchString,
                        CurrentPage = page,
                        type = type,
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

        public Models.Customer GetById(int Id)
        {
            try
            {
                return _context.Customer.FirstOrDefault(m => m.CustomerID == Id);
            }
            catch (System.Exception ex)
            {
                return new Models.Customer();
            }
        }

        public Models.Customer Login(ViewLogin viewLogin)
        {
            var cus = _context.Customer.Where(u => u.Email.Equals(viewLogin.UserName)
            && u.Password.Equals(_EncodeHelper.Encode(viewLogin.Password))).FirstOrDefault();

            return cus;
        }

        public int UpdateCustomer(Models.Customer customer)
        {
            int status = 0;
            try
            {
                // Lấy đối tượng từ database dựa vào id
                var existingCus = _context.Customer.FirstOrDefault(m => m.CustomerID == customer.CustomerID);

                // Kiểm tra xem đối tượng có tồn tại trong database không
                if (existingCus == null)
                {
                    return 0;
                }

                // Cập nhật thông tin của đối tượng 
                existingCus.CustomerID = customer.CustomerID;
                existingCus.FullName = customer.FullName;
                existingCus.Gender = customer.Gender;
                existingCus.Email = customer.Email;
                existingCus.DateOfBirth = customer.DateOfBirth;
                existingCus.PhoneNumber = customer.PhoneNumber;
                existingCus.Address = customer.Address;
                existingCus.Locked = customer.Locked;
                existingCus.Note = customer.Note;
                existingCus.Point = customer.Point;
                if (customer.Password == null)
                {
                    existingCus.Password = existingCus.Password;
                }
                // Lưu thay đổi vào database
                _context.SaveChanges();
                status = customer.CustomerID;
            }
            catch (System.Exception ex)
            {
                status = 0;
            }
            return status;
        }
    }
}
