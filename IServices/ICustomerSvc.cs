﻿using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;

namespace Assignment_CS5.IServices
{
    public interface ICustomerSvc
    {
        public PaginationViewModel GetAll(string type, string searchString, int page);
        public Customer GetById(int Id);
        public int AddCustomer(Customer customer);
        public int UpdateCustomer(Customer customer);
        Customer Login(ViewLogin viewLogin);
        public int IsFieldExist(Customer customer);
		public int ChangePassword(int cusId,ChangePassword changePassword);

	}
}
