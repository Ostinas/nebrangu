using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nebrangu.Models;

namespace nebrangu.Controllers
{
    public class OrderController : Controller
    {
        private readonly nebranguContext _context;
        private IHttpContextAccessor _httpContextAccessor;

        public OrderController(nebranguContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            return _context.Order != null ?
                        View(await _context.Order.ToListAsync()) :
                        Problem("Entity set 'nebranguContext.Order'  is null.");
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveChanges([Bind("Id,OrderDate,Sum,DeliveryAddress,DeliveryCity,DeliveryPostalCode,isPaid")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,Sum,DeliveryAddress,DeliveryCity,DeliveryPostalCode,isPaid")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'nebranguContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
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
                Id = productId,
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
    }
}
