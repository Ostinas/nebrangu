using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nebrangu.Models;
using nebrangu.Repositories;

namespace nebrangu.Controllers
{
    public class OrderController : Controller
    {
        private readonly nebranguContext _context;
        private OrdersRepo _repo;
        private IHttpContextAccessor _httpContextAccessor;

        public OrderController(nebranguContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _repo = new OrdersRepo(context);
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            return _context.Order != null ?
                        View(await _context.Order.ToListAsync()) :
                        Problem("Entity set 'nebranguContext.Order'  is null.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirmOrder([Bind("OrderDate, Sum, User, Status, isPaid, DeliveryType, DeliveryAddress, DeliveryCity, DeliveryPostalCode, PaymentMethod, OrderProducts")] Order order)
        {
            order.OrderProducts.ForEach(orderProduct => orderProduct.Product = _context.Products.Find(orderProduct.Product.Id));
            order.OrderProducts.ForEach(product => product.Order = order);
            order.DeliveryType = _context.Delivery_Types.Find(order.DeliveryType.Id);
            order.PaymentMethod = _context.Payment_Methods.Find(order.PaymentMethod.Id);
            order.Status = _context.Order_Statuses.Find(1);
            order.User = _context.Users.Find(1);

            await _repo.Create(order);
            return View("OrderInformationPage", order);
        }

        public IActionResult Create()
        {
            ProductController productController = new ProductController(_context, _httpContextAccessor);
            Dictionary<int, decimal> cartProductPrices = productController.GetCartPrices();

            double orderSum = Convert.ToDouble(CalculateOrderSum(cartProductPrices));

            Order order = new Order
            {
                OrderDate = DateTime.Now,
                Sum = orderSum,
                User = _context.Users.Find(1),
                Status = _context.Order_Statuses.Find(1),
                isPaid = false 
            };

            List<Order_Product> orderProducts = cartProductPrices.Keys.Select(productId => new Order_Product
            {
                Product = _context.Products.Find(productId),
                Order = order
            }).ToList();
            order.OrderProducts = orderProducts;

            return View("OrderCreatePage", order);
        }

        public decimal CalculateOrderSum(Dictionary<int, decimal> cartProductPrices)
        {
            return cartProductPrices.Values.Sum();
        }

        public IActionResult GetPaymentDetails(int orderId)
        {
            int paymentMethod = _repo.GetPaymentMethod(orderId);
            
            if (paymentMethod == 1)
            {
                string sellerBankAccount = new UserController(_context).GetSellerBankAccount(1);
                return Content("Atlikite bankinį pavedimą į šią sąskaitą: " + sellerBankAccount);
            }
            return Content("Apmokėjimas atsiemimo metu.");
        }
    }
}
