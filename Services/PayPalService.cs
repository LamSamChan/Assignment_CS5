﻿using System.Net;
using Assignment_CS5.Models;
using Assignment_CS5.ViewModels;
using PayPal.Core;
using PayPal.v1.Payments;
using Assignment_CS5.IServices;
using Microsoft.EntityFrameworkCore;
using Assignment_CS5.Database;

namespace Assignment_CS5.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _context;

        private const double ExchangeRate = 22_863.0;

        public PayPalService(IConfiguration configuration, MyDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public PaginationViewModel GetAll(string searchString, int page)
        {
            var list = _context.PaymentResponses.ToList();
            try
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    list = list.Where(p => p.PaymentId.ToLower().Contains(searchString.ToLower()) || p.PayerId.ToLower().Contains(searchString.ToLower())).ToList();
                }
                int pageSize = 5;
                var pagedProducts = list.Skip((page - 1) * pageSize).Take(pageSize); // Lấy các sản phẩm cho trang hiện tại

                var viewModel = new PaginationViewModel
                {
                    PaymentResponse = pagedProducts,
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
        public string AddPaymentRespone(PaymentResponse paymentResponse)
        {
            string status = "";
            try
            {

                _context.Add(paymentResponse);
                _context.SaveChanges();
                status = paymentResponse.PaymentId;
            }
            catch
            {
                status = null;
            }
            return status;
        }
        public static double ConvertVndToDollar(double vnd)
        {
            var total = Math.Round(vnd / ExchangeRate, 2);

            return total;
        }

        public async Task<string> CreatePaymentUrl(List<ViewCart> model, double total)
        {
            // var envProd = new LiveEnvironment(_configuration["PaypalProduction:ClientId"],
            //     _configuration["PaypalProduction:SecretKey"]);

            var envSandbox =
                new SandboxEnvironment(_configuration["Paypal:ClientId"], _configuration["Paypal:SecretKey"]);
            var client = new PayPalHttpClient(envSandbox);
            var paypalOrderId = DateTime.Now.Ticks;
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            var itemList = new ItemList();
            itemList.Items = new List<Item>();

            foreach (var cartItem in model)
            {
                var item = new Item()
                {
                    Name = " | Name: " + cartItem.Menu.Name,
                    Currency = "USD",
                    Price = ConvertVndToDollar(cartItem.Menu.Price).ToString(),
                    Quantity = cartItem.Quantity.ToString(),
                };

                itemList.Items.Add(item);
            }

            var payment = new PayPal.v1.Payments.Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = ConvertVndToDollar(total).ToString(),
                            Currency = "USD",
                            Details = new AmountDetails
                            {
                                Tax = "0",
                                Shipping = "0",
                                Subtotal = ConvertVndToDollar(total).ToString(),
                            }
                        },
                        ItemList =itemList,
                        InvoiceNumber = paypalOrderId.ToString()
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    ReturnUrl =
                        $"{urlCallBack}?payment_method=PayPal&success=1&order_id={paypalOrderId}",
                    CancelUrl =
                        $"{urlCallBack}?payment_method=PayPal&success=0&order_id={paypalOrderId}"
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };
            var request = new PaymentCreateRequest();
            request.RequestBody(payment);

            var paymentUrl = "";
            var response = await client.Execute(request);
            var statusCode = response.StatusCode;

            if (statusCode is not (HttpStatusCode.Accepted or HttpStatusCode.OK or HttpStatusCode.Created))
                return paymentUrl;
            
            var result = response.Result<Payment>();
            using var links = result.Links.GetEnumerator();

            while (links.MoveNext())
            {
                var lnk = links.Current;
                if (lnk == null) continue;
                if (!lnk.Rel.ToLower().Trim().Equals("approval_url")) continue;
                paymentUrl = lnk.Href;
            }

            return paymentUrl;

        }

        public PaymentResponse PaymentExecute(IQueryCollection collections)
        {
            var response = new PaymentResponse();

            foreach (var (key, value) in collections)
            {

                if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("order_id"))
                {
                    response.OrderId = value;
                }

                if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("payment_method"))
                {
                    response.PaymentMethod = value;
                }

                if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("success"))
                {
                    response.Success = Convert.ToInt32(value) > 0;
                }

                if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("paymentid"))
                {
                    response.PaymentId = value;
                }

                if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("payerid"))
                {
                    response.PayerId = value;
                }
            }

            return response;
        }
    }
}