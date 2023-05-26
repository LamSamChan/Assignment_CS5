using Assignment_CS5.Database;
using Assignment_CS5.Helpers;
using Assignment_CS5.IServices;
using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
namespace Assignment_CS5.Services
{
    public class CustomerSvc : ICustomerSvc
    {
        private readonly MyDbContext _context;
        public CustomerSvc(MyDbContext context)
        {
            this._context = context;
        }
        public int AddOrder(Models.Customer customer)
        {
            throw new NotImplementedException();
        }

        public PaginationViewModel GetAll(string searchString, int page)
        {
            throw new NotImplementedException();
        }

        public Models.Customer GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Models.Customer Login(ViewLogin viewLogin)
        {
            throw new NotImplementedException();
        }

        public int UpdateOrder(Models.Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
