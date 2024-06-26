﻿using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;

namespace Assignment_CS5.IServices
{
	public interface IOrderSvc
	{
		public PaginationViewModel GetAll(string type,string searchString,DateTime searchDate, int page);
        public PaginationViewModel GetAllForCus(int cusId,string searchString, DateTime searchDate, int page);

        public Order GetById(string Id);

		public string AddOrder(Order order);
		public string UpdateOrder(Order order);
		public List<OrderDetails> GetOrderDetails(string id);

    }
}
