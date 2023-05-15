using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nebrangu.Models;
using nebrangu.Repositories;
using Newtonsoft.Json;

namespace nebrangu.Controllers
{
    public class ProductController : Controller
    {
        private readonly nebranguContext _context;
        private ProductsRepo _repo;
        private IHttpContextAccessor _httpContextAccessor;

        public ProductController(nebranguContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _repo = new ProductsRepo(_context);
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _repo.GetAll();
            var userController = new UserController(_context,_httpContextAccessor);
            ProductViewModel viewModel = new ProductViewModel();

            var sortedProducts = products.OrderBy(p=>p.Name).ToList();

            if (userController != null)
            {
                List<Product> recommendedProducts = await userController.CalculateEmotionCoefficientAsync();

                viewModel = new ProductViewModel
                {
                    Products = sortedProducts,
                    RecommendedProducts = recommendedProducts
                };
            }



            return View("ProductPage", viewModel);
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var products = await _repo.GetAll();
            return products;
        }

        public List<Product> FilterBySeasonAndWeather(List<int> category, List<string> sAw, List<Product> Products, double emotionCoefficient)
        {

            // Define a weight for each sorting variable
            const double CATEGORY_WEIGHT = 0.2;
            const double SEASON_WEIGHT = 0.4;
            const double WEATHER_WEIGHT = 0.4;

            // Define a dictionary to map each product to a score based on the sorting variables
            Dictionary<Product, double> productScores = new Dictionary<Product, double>();

            foreach (Product product in Products)
            {
                double score = 0;

                if (product != null)
                {
                    // Calculate the score for the category variable
                    if (product.CategoryId == category[0])
                    {
                        score += CATEGORY_WEIGHT;
                    }

                    // Calculate the score for the season variable
                    if (product.Season.Name == sAw[0])
                    {
                        score += SEASON_WEIGHT;
                    }

                    // Calculate the score for the weather variable
                    if (product.Weather.Name == sAw[1])
                    {
                        score += WEATHER_WEIGHT;
                    }

                    productScores[product] = score;
                }
            }

            // Sort the products by their scores
            List<Product> sortedProducts = productScores.OrderByDescending(x => x.Value * emotionCoefficient).Select(x => x.Key).ToList();

            return sortedProducts;
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _repo.GetById((int)id);
            if (product == null)
            {
                return NotFound();
            }

            return View("ProductDetailsPage", product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name");
            ViewData["SeasonId"] = new SelectList(_context.Seasons, "Id", "Name");
            ViewData["WeatherId"] = new SelectList(_context.Weathers, "Id", "Name");
            return View("ProductCreationPage");
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Product product)
        {
            if (CheckDetails(product))
            {
                return RedirectToAction(nameof(CreateConfirm), product);
            }
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            //ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name", product.ManufacturerId);
            //ViewData["SeasonId"] = new SelectList(_context.Seasons, "Id", "Name", product.SeasonId);
            //ViewData["WeatherId"] = new SelectList(_context.Weathers, "Id", "Name", product.WeatherId);
            await _repo.Create(product);
            return RedirectToAction("Index");
        }
        
        // POST: Products/CreateConfirm
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirm([FromForm] Product product)
        {
            return View("CreateConfirm", product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }
            
            var product = await _repo.GetById((int)id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name", product.ManufacturerId);
            ViewData["SeasonId"] = new SelectList(_context.Seasons, "Id", "Name", product.SeasonId);
            ViewData["WeatherId"] = new SelectList(_context.Weathers, "Id", "Name", product.WeatherId);
            return View("ProductEditPage", product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Rating,RatingCount,CategoryId,ManufacturerId,WeatherId,SeasonId,OriginCountry")] Product product)
        {
            bool valid = CheckChanges(id, product);

            if (valid)
            {
                try
                {
                    await _repo.Update(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name", product.ManufacturerId);
            ViewData["SeasonId"] = new SelectList(_context.Seasons, "Id", "Name", product.SeasonId);
            ViewData["WeatherId"] = new SelectList(_context.Weathers, "Id", "Name", product.WeatherId);
            return RedirectToAction("Index");
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            await _repo.Delete(id);
            //var product = await _context.Products
            //    .Include(p => p.Category)
            //    .Include(p => p.Manufacturer)
            //    .Include(p => p.Season)
            //    .Include(p => p.Weather)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (product == null)
            //{
            //    return NotFound();
            //}

            //return View(product);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeletePage(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Season)
                .Include(p => p.Weather)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View("ProductDeletionPage", product);
            //return RedirectToAction("Index");
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'nebranguContext.Products'  is null.");
            }
            var product = await _repo.GetById(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _repo.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        //TODO: change to private
        public bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public bool CheckChanges(int id, Product product)
        {
            return id == product.Id;
        }

        public bool CheckDetails(Product product)
        {
            return product.Name == null ? true : false;
        }

        public async Task<IActionResult> AddProductToCart(int id)
        {
            bool isInCart = CheckIfProductIsInCart(id);
            
            if (!isInCart)
            {
                SaveToCookies(id);
                return NoContent();
            }
            else
            {
                string cartCookieJson = _httpContextAccessor.HttpContext.Request.Cookies["cart"];
                Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson);

                ChangeProductCountInCookies(id, cart[id] + 1);

                return NoContent();
            }
        }

        public bool CheckIfProductIsInCart(int id)
        {
            string cartCookieJson = _httpContextAccessor.HttpContext.Request.Cookies["cart"];
            Dictionary<int, int> cart = !string.IsNullOrEmpty(cartCookieJson) ? JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson) : new Dictionary<int, int>();

            return cart.ContainsKey(id) && cart[id] >= 0;
        }

        public bool SaveToCookies(int id)
        {
            string cartCookieJson = _httpContextAccessor.HttpContext.Request.Cookies["cart"];
            Dictionary<int, int> cart = !string.IsNullOrEmpty(cartCookieJson) ? JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson) : new Dictionary<int, int>();

            cart.Add(id, 1);
            string cartCookieJsonNew = JsonConvert.SerializeObject(cart);
            _httpContextAccessor.HttpContext.Response.Cookies.Append("cart", cartCookieJsonNew);
            
            return true;
        }

        public async Task<IActionResult> Krepselis()
        {
            string cartCookieJson = _httpContextAccessor.HttpContext.Request.Cookies["cart"];

            Dictionary<int, int> cart = !string.IsNullOrEmpty(cartCookieJson) ? JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson) : new Dictionary<int, int>();

            var products = from p in _context.Products
                           where cart.Keys.Contains(p.Id)
                           select p;

            return View("ShoppingCartPage", products);
        }

        public IActionResult SaveCartProductCount(int id, int newCount)
        {
            StocksRepo stocks = new StocksRepo(_context);
            int productStockCount = stocks.GetProductStock(id);

            // Get product count from cookies
            string cartCookieJson = _httpContextAccessor.HttpContext.Request.Cookies["cart"];
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson);

            if (productStockCount >= newCount)
            {
                ChangeProductCountInCookies(id, newCount);
                return NoContent();
            }
            return BadRequest("Nepakankamas likutis!");
		}

        public int ChangeProductCountInCookies(int id, int newCount)
        {
            string cartCookieJson = _httpContextAccessor.HttpContext.Request.Cookies["cart"];
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson);
            cart[id] = newCount;
            
            string cartCookieJsonNew = JsonConvert.SerializeObject(cart);
            _httpContextAccessor.HttpContext.Response.Cookies.Append("cart", cartCookieJsonNew);

            return newCount;
        }

        public Task<IActionResult> DeleteProductFromCart(int id)
        {
            RemoveProductFromCookies(id);
            return Krepselis();
        }

        public IResponseCookies RemoveProductFromCookies(int id)
        {
            string cartCookieJson = _httpContextAccessor.HttpContext.Request.Cookies["cart"];
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson);
            cart.Remove(id);

            string cartCookieJsonNew = JsonConvert.SerializeObject(cart);
            _httpContextAccessor.HttpContext.Response.Cookies.Append("cart", cartCookieJsonNew);
            return _httpContextAccessor.HttpContext.Response.Cookies;
        }
        
        public Dictionary<int, decimal> GetCartPrices()
        {
            string cartCookieJson = _httpContextAccessor.HttpContext.Request.Cookies["cart"];
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson);
            
            Dictionary<int, decimal> productPrices = _repo.GetProductPrices(cart);
            return productPrices;
        }
    }
}
