using Assignment_CS5.Database;
using Assignment_CS5.IServices;
using Assignment_CS5.Models;

namespace Assignment_CS5.Services
{
	public class OrderDetailSvc : IOrderDetailSvc
	{
		private readonly MyDbContext _context;
		
		public OrderDetailSvc(MyDbContext context)
		{
			_context = context;
		}
		public int AddOrderDetail(OrderDetails orderDetails)
		{
			int status = 0;
			try
			{

				_context.Add(orderDetails);
				_context.SaveChanges();
				status = orderDetails.OrderDetailId;
			}
			catch
			{
				status = 0;
			}
			return status;
		}
	}
}
