using Assignment_CS5.Database;
using Assignment_CS5.IServices;
using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Assignment_CS5.Services
{
    public class MenuSvc : IMenuSvc
    {
        private readonly MyDbContext _context;

        public MenuSvc(MyDbContext context)
        {
            this._context = context;
        }
        public int AddProduct(Menu product)
        {
            int status = 0;
            try
            {
                
                _context.Add(product);
                _context.SaveChanges();
                status = product.ProductId;
            }
            catch
            {
                status = 0;
            }
            return status;
        }

        public string DeleteProduct(int Id)
        {
            try
            {
                var product = _context.Menus.FirstOrDefault(x => x.ProductId == Id);
                _context.Menus.Remove(product);
                _context.SaveChanges();
                return "Product deleted successfully.";
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
        }

        public PaginationViewModel GetAll(string searchString, int page)
        {
            var list = _context.Menus.ToList();
            try
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    list = list.Where(p => p.Name.ToLower().Contains(searchString.ToLower())).ToList();
                }
                int pageSize = 5;
                var pagedProducts = list.Skip((page - 1) * pageSize).Take(pageSize); // Lấy các sản phẩm cho trang hiện tại

                var viewModel = new PaginationViewModel
                {
                    Menus = pagedProducts,
                    PaginationInfo = new PaginationInfo
                    {
                        SearchKeyword = searchString,
                        CurrentPage = page,
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


        public Menu GetById(int Id)
        {
            try
            {
               return _context.Menus.FirstOrDefault(m => m.ProductId == Id);
            }
            catch (System.Exception ex)
            {
                return new Menu();
            }
        }

        public int UpdateProduct(Menu product)
        {
            int status = 0;
            try
            {
                // Lấy đối tượng từ database dựa vào id
                var existingPro = _context.Menus.FirstOrDefault(m => m.ProductId == product.ProductId);

                // Kiểm tra xem đối tượng có tồn tại trong database không
                if (existingPro == null)
                {
                    return 0;
                }

                // Cập nhật thông tin của đối tượng 
                existingPro.ProductId = product.ProductId;
                existingPro.Name = product.Name;
                existingPro.Price = product.Price;
                existingPro.Category = product.Category;
                existingPro.Image = product.Image;
                existingPro.Description = product.Description;
                existingPro.Status = product.Status;
                // Lưu thay đổi vào database
                _context.SaveChanges();
                status = product.ProductId;
            }
            catch (System.Exception ex)
            {
                status = 0;
            }
            return status;
        }

        public List<Menu> GetAllMenu()
        {
            try
            {
                return _context.Menus.ToList();
            }
            catch (Exception)
            {

                return new List<Menu>();
            }
            
        }
    }
}
