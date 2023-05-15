using Microsoft.EntityFrameworkCore;
using nebrangu.Models;

namespace nebrangu.Repositories
{
    public class ProductsRepo
    {
        private readonly nebranguContext _context;

        public ProductsRepo(nebranguContext context)
        {
            _context = context;
        } 

        public async Task<List<Product>> GetAll()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Season)
                .Include(p => p.Weather)
                .ToListAsync();
        }

        public async Task<Product> GetById(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Season)
                .Include(p => p.Weather)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> Create(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> Update(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public Dictionary<int, decimal> GetProductPrices(Dictionary<int, int> cart)
        {
            Dictionary<int, decimal> prices = new Dictionary<int, decimal>();

            var products = from p in _context.Products
                           where cart.Keys.Contains(p.Id)
                           select p;

            products.ToList().ForEach(p => prices.Add(p.Id, p.Price * cart[p.Id]));
            return prices;
        }
    }
}
