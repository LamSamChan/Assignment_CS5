﻿using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;

namespace Assignment_CS5.IServices
{
	public interface IOrderSvc
	{
		public PaginationViewModel GetAll(string type,string searchString,DateTime orderDate, int page);
		public Order GetById(int Id);
		public int AddOrder(Order order);
		public int UpdateOrder(Order order);
	}
}
