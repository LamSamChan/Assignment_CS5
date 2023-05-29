using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;

namespace Assignment_CS5.IServices
{
    public interface IEmployeeSvc
    {
        public PaginationViewModel GetAll(string type, string sreachString,int page);
        public Employee GetById(int Id);
        public int AddEmployee(Employee employee);
        public int UpdateEmployee(Employee employee);
        public Employee Login (ViewLogin viewLogin);
        public int IsFieldExist(Employee employee);
    }
}
