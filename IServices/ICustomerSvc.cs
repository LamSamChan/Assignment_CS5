using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;

namespace Assignment_CS5.IServices
{
    public interface ICustomerSvc
    {
        public PaginationViewModel GetAll(string searchString, int page);
        public Customer GetById(int Id);
        public int AddOrder(Customer customer);
        public int UpdateOrder(Customer customer);
        Customer Login(ViewLogin viewLogin);
    }
}
