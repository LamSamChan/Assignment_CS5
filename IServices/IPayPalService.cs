using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;

namespace Assignment_CS5.IServices
{
    public interface IPayPalService
    {
        Task<string> CreatePaymentUrl(List<ViewCart> model, double total);
        PaymentResponse PaymentExecute(IQueryCollection collections);
        public string AddPaymentRespone(PaymentResponse paymentResponse);
        public PaginationViewModel GetAll(string searchString, int page);


    }
}
