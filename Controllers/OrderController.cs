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

        public async Task<List<int>> CheckOrderHistory()
        {
            List<Order> orders = await _repo.GetAll();
            int category = new int();

            if (orders == null)
            {
                //Get Every users categories..
            }
            else
            {
                category = GetMostLikedCategory(orders);
            }

            List<int> categories = new List<int>();
            categories.Add(category);

            return categories;

        }

        private int GetMostLikedCategory(List<Order> orders)
        {
            List<int> usedCategories = new List<int>();

            foreach (var order in orders)
            {
                if(order.OrderProducts != null)
                    foreach (var item in order.OrderProducts)
                    {
                        if (item.Product.CategoryId != null)
                        {
                            usedCategories.Add(item.Product.CategoryId);
                        }
                    }
            }

            if (usedCategories.Count == 0)
                return 1;

            int mostFrequent = usedCategories.GroupBy(s => s)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .First();

            return mostFrequent;
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
                string sellerBankAccount = new UserController(_context, _httpContextAccessor).GetSellerBankAccount(1);
                return Content("Atlikite bankinį pavedimą į šią sąskaitą: " + sellerBankAccount);
            }
            return Content("Apmokėjimas atsiemimo metu.");
        }

        public async Task<List<Order>> GetOrderList(int userId)
        {
            var orderList = await _repo.GetUserOrders(userId);
            return orderList;
        }

        public async Task<IActionResult> OpenOrder(int orderId)
        {
            var order = await _repo.GetOrderDetails(orderId);
            return View("OrderInformationPage", order);
        }

        public async Task<IActionResult> OpenOrderStatistics()
        {
            var orders = await _repo.GetAll();
            int doneOrderCount = await CalculateDoneOrderCount();
            int lostDisputeCount = await CalculateLostDisputes();
            double sum = await CalculateOrderSum();
            int activeDiscounts = CalculateActiveDiscounts();
            List<Order> orderList = await CreateNotSentOrderList();


            OrderStatisticsViewModel viewModel = new OrderStatisticsViewModel()
            {
                DoneOrderCount = doneOrderCount,
                LostDisputesCount = lostDisputeCount,
                OrderSum = sum,
                ActiveDiscounts = activeDiscounts,
                NotSentOrders = orderList
            };
            return View("OrderStatisticsPage", viewModel);
        }

        public async Task<int> CalculateDoneOrderCount()
        {
            int count = 0;
            var orders = await _repo.GetAll();

            foreach (var order in orders)
            {
                if (order.Status.Name == "Pristatyta")
                {
                    count++;
                }
            }

            return count;
        }

        public async Task<int> CalculateLostDisputes()
        {
            int count = 0;

            //MANTAS SITA RELIZUOS SAKE
            return count;
        }

        public async Task<double> CalculateOrderSum()
        {
            double sum = 0;
            var orders = await _repo.GetAll();

            foreach(var order in orders)
            {
                sum += order.Sum;
            }

            return sum;
        }

        public int CalculateActiveDiscounts()
        {
            return 0;
        }

        public async Task<List<Order>> CreateNotSentOrderList()
        {
            List<Order> orderList = new List<Order>();

            var orders = await _repo.GetAll();

            foreach( var order in orders)
            {
                if(order.Status.Name == "Sukurta" || order.Status.Name == "Laukiama apmokėjimo" || order.Status.Name == "Apmokėta" || order.Status.Name == "Ruošiama")
                {
                    orderList.Add(order);
                }
            }

            return orderList;
        }
    }
}
