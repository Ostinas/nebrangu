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
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null || _repo.GetById((int)id) == null)
            {
                return NotFound();
            }

            var product = await _repo.GetById((int)id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name");
            ViewData["SeasonId"] = new SelectList(_context.Seasons, "Id", "Name");
            ViewData["WeatherId"] = new SelectList(_context.Weathers, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Rating,RatingCount,CategoryId,ManufacturerId,WeatherId,SeasonId,OriginCountry")] Product product)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(CreateConfirm), product);
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name", product.ManufacturerId);
            ViewData["SeasonId"] = new SelectList(_context.Seasons, "Id", "Name", product.SeasonId);
            ViewData["WeatherId"] = new SelectList(_context.Weathers, "Id", "Name", product.WeatherId);
            return View(product);
        }
        
        // POST: Products/CreateConfirm
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirm([Bind("Id,Name,Description,Price,Rating,RatingCount,CategoryId,ManufacturerId,WeatherId,SeasonId,OriginCountry")] Product product)
        {
            await _repo.Create(product);
            return RedirectToAction(nameof(Index));
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
            return RedirectToAction("Index", "Home");
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
            return View(product);
        }

        public bool CheckChanges(int id, Product product)
        {
            return ModelState.IsValid && id == product.Id;
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
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

            return View(product);
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

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
