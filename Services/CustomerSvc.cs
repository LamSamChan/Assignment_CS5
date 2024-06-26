﻿using Assignment_CS5.Database;
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
                if (IsFieldExist(customer) != 0)
                {
                    return IsFieldExist(customer);
                }
                else
                {
                    customer.Password = _EncodeHelper.Encode(customer.Password);
                    _context.Add(customer);
                    _context.SaveChanges();
                    status = customer.CustomerID;
                }
                
            }
            catch
            {
                status = 0;
            }
            return status;
        }

        public PaginationViewModel GetAll(string type, string searchString, int page)
        {
            var list = _context.Customer.ToList();
            try
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    if (type == "ID")
                    {
                        list = list.Where(c => c.CustomerID.ToString().Contains(searchString.ToLower())).ToList();
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
            var pw = _EncodeHelper.Encode(viewLogin.Password);
            var email = viewLogin.UserName;
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
        public int IsFieldExist(Models.Customer customer)
        {
            string phoneNumber = null;
            
            if (customer.PhoneNumber.StartsWith("+84"))
            {
                phoneNumber = customer.PhoneNumber.Substring(3);
            }
            else if (customer.PhoneNumber.StartsWith("0"))
            {
                phoneNumber = customer.PhoneNumber.Substring(1);
            }

            foreach (var cus in _context.Customer.ToList())
            {
                string existPhoneNumber = null;
                if (cus.PhoneNumber.StartsWith("+84"))
                {
                    existPhoneNumber = cus.PhoneNumber.Substring(3);
                }
                else if (cus.PhoneNumber.StartsWith("0"))
                {
                    existPhoneNumber = cus.PhoneNumber.Substring(1);
                }
                
                 if (customer.Email == cus.Email)
                {
                    return -1;
                }
                else if (phoneNumber == existPhoneNumber)
                {
                    return -2;
                }
            }

            return 0;
        }
		public int ChangePassword(int cusId, ChangePassword changePassword)
        {
            var cus = GetById(cusId);    
            if(cus.Password != _EncodeHelper.Encode(changePassword.Password))
            {
                return 0;
            }
            else
            {
                cus.Password = _EncodeHelper.Encode(changePassword.NewPassword);
                UpdateCustomer(cus);
                return 1;

            }
        }

	}
}
