using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;

namespace Assignment_CS5.IServices
{
    public interface IMenuSvc
    {
        public PaginationViewModel GetAll(string searchString, int page);
        public Menu GetById(int Id);
        public int AddProduct(Menu product);
        public int UpdateProduct(Menu product);
        public string DeleteProduct(int Id);
    }
}
