using Assignment_CS5.Database;
using Assignment_CS5.IServices;
using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;

namespace Assignment_CS5.Services
{
	
	public class OrderSvc : IOrderSvc
	{
		private readonly MyDbContext _context;
		public OrderSvc(MyDbContext context)
		{
			_context = context;
		}
		public int AddOrder(Order order)
		{
			int status = 0;
			try
			{

				_context.Add(order);
				_context.SaveChanges();
				status = order.OrderId;
			}
			catch
			{
				status = 0;
			}
			return status;
		}

		public PaginationViewModel GetAll(string type,string searchString, DateTime searchDate, int page)
		{
			var list = _context.Orders.OrderByDescending(x => x.OrderDate)
				.Include(x => x.Customer)
				.Include(x => x.OrderDetails)
				.ToList();
			try
			{
				if (!string.IsNullOrEmpty(searchString))
				{
					if(type == "PN")
					{
						if (!string.IsNullOrEmpty(searchDate.ToString()))
						{
                            list = list.Where(p => p.Customer.PhoneNumber.Contains(searchString.ToLower()) 
							&& p.OrderDate.ToString().Contains(searchDate.ToString("yyyy/MM/dd"))).ToList();
						}
						else
						{
                            list = list.Where(p => p.Customer.PhoneNumber.Contains(searchString.ToLower())).ToList();
                        }
						
					}
					else
					{
                        if (!string.IsNullOrEmpty(searchDate.ToString()))
                        {
                            list = list.Where(p => p.Customer.CustomerID.ToString().Contains(searchString.ToLower())
                            && p.OrderDate.ToString().Contains(searchDate.ToString("yyyy/MM/dd"))).ToList();
                        }
                        else
                        {
                            list = list.Where(p => p.Customer.CustomerID.ToString().Contains(searchString.ToLower())).ToList();
                        }
                       
					}
					
				}
				int pageSize = 5;
				var pagedOrders = list.Skip((page - 1) * pageSize).Take(pageSize); // Lấy các sản phẩm cho trang hiện tại

				var viewModel = new PaginationViewModel
				{
					Orders = pagedOrders,
					PaginationInfo = new PaginationInfo
					{
						SearchKeyword = searchString,
						CurrentPage = page,
						type=type,
						SearchDate = searchDate,
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

		public Order GetById(int Id)
		{
			try
			{
				return _context.Orders.Where(o => o.OrderId == Id)
					.Include(o => o.Customer)
					.Include(o => o.OrderDetails)
					.ThenInclude(o => o.Menu)
					.FirstOrDefault();
			}
			catch (System.Exception ex)
			{
				return new Order();
			}
		}

		public int UpdateOrder(Order order)
		{
			int status = 0;
			try
			{
				// Lấy đối tượng từ database dựa vào id
				var existingOrder = _context.Orders.FirstOrDefault(m => m.OrderId == order.OrderId);

				// Kiểm tra xem đối tượng có tồn tại trong database không
				if (existingOrder == null)
				{
					return 0;
				}

				// Cập nhật thông tin của đối tượng 
				existingOrder.OrderId = order.OrderId;
				existingOrder.CustomerId = order.CustomerId;
				existingOrder.OrderDate = order.OrderDate;
				existingOrder.Total = order.Total;
				existingOrder.Status = order.Status;
				existingOrder.Note = order.Note;
				// Lưu thay đổi vào database
				_context.SaveChanges();
				status = order.OrderId;
			}
			catch (System.Exception ex)
			{
				status = 0;
			}
			return status;
		}
	}
}
