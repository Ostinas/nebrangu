using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nebrangu;
using nebrangu.Models;
using nebrangu.Repositories;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Mozilla;

namespace nebrangu.Controllers
{
    public class ProductsController : Controller
    {
        private readonly nebranguContext _context;
        private ProductsRepo _repo;

        public ProductsController(nebranguContext context)
        {
            _context = context;
            _repo = new ProductsRepo(_context);
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _repo.GetAll();
            return View("ProductPage", products);
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

        public async Task<IActionResult> Krepselis()
        {
            string cartCookieJson = HttpContext.Request.Cookies["cart"];

            Dictionary<int, int> cart = !string.IsNullOrEmpty(cartCookieJson) ? JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson) : new Dictionary<int, int>();

            var products = from p in _context.Products
                           where cart.Keys.Contains(p.Id)
                           select p;

            return View("ShoppingCartPage", products);
        }

        public int SaveCartProductCount(int id, int newCount)
        {
            StocksRepo stocks = new StocksRepo(_context);
            int productStockCount = stocks.GetProductStock(id);

            // Get product count from cookies
            string cartCookieJson = HttpContext.Request.Cookies["cart"];
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson);

            if (productStockCount >= newCount)
            {
                return ChangeProductCountInCookies(id, newCount);
            }
            return -1;
        }

        public int ChangeProductCountInCookies(int id, int newCount)
        {
            string cartCookieJson = HttpContext.Request.Cookies["cart"];
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson);
            cart[id] = newCount;
            
            string cartCookieJsonNew = JsonConvert.SerializeObject(cart);
            HttpContext.Response.Cookies.Append("cart", cartCookieJsonNew);

            return newCount;
        }

        public IResponseCookies DeleteProductFromCart(int id)
        {
            return RemoveProductFromCookies(id);
        }

        public IResponseCookies RemoveProductFromCookies(int id)
        {
            string cartCookieJson = HttpContext.Request.Cookies["cart"];
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson);
            cart.Remove(id);

            string cartCookieJsonNew = JsonConvert.SerializeObject(cart);
            HttpContext.Response.Cookies.Append("cart", cartCookieJsonNew);
            return HttpContext.Response.Cookies;
        }
    }
}
