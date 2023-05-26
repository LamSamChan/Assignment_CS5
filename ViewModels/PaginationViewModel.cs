using Assignment_CS5.Models;

namespace Assignment_CS5.ViewModels
{
    public class PaginationViewModel
    {
        public IEnumerable<Menu> Menus { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
		public IEnumerable<Order> Orders { get; set; }
		public IEnumerable<OrderDetails> OrderDetails { get; set; }
		public IEnumerable<Customer> Customers { get; set; }
		public PaginationInfo PaginationInfo { get; set; }

    }
    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public string? SearchKeyword { get; set; }
        public DateTime? SearchDate { get; set; }
        public string type { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
