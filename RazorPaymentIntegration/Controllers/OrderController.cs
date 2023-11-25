using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using RazorPaymentIntegration.Models;

namespace RazorPaymentIntegration.Controllers
{
    public class OrderController : Controller
    {
        [BindProperty]
        public OrderEntity _OrderDetails { get; set; }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult InitiateOrder()
        {
            string key = "rzp_test_GPY41eziOXjwIK";
            string secret = "G0E908O2SPG69oGm3HfKeQyX";

            Random _random = new Random();
            string TransactionId = _random.Next(0, 10000).ToString();

            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", Convert.ToDecimal(_OrderDetails.TotalAmount)); // this amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("receipt", TransactionId);

            RazorpayClient client = new RazorpayClient(key, secret);
            Razorpay.Api.Order order = client.Order.Create(input);

            ViewBag.orderid = order["id"].ToString();

            return View("Payment",_OrderDetails);
        }

        public IActionResult Payment(string razorpay_payment_id, string razorpay_order_id,
            string razorpay_signature)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("razorpay_payment_id", razorpay_payment_id);
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature);

            Utils.verifyPaymentSignature(attributes);

            OrderEntity orderdetalis = new OrderEntity();

            orderdetalis.TransactionId = razorpay_payment_id;
            orderdetalis.OrderId = razorpay_order_id;

            return View("PaymentSucces", orderdetalis);
        }
    }
}
