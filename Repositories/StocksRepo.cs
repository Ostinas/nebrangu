using Microsoft.EntityFrameworkCore;
using nebrangu.Models;

namespace nebrangu.Repositories
{
    public class StocksRepo
    {
        private readonly nebranguContext _context;

        public StocksRepo(nebranguContext context)
        {
            _context = context;
        }

        public async Task<List<Stock>> GetAll()
        {
            return await _context.Stocks.ToListAsync();
        }

        public async Task<Stock> GetById(int id)
        {
            return await _context.Stocks
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public int GetProductStock(int id)
        {
            return _context.Stocks
                .Where(s => s.Product.Id == id)
                .Select(s => s.Count)
                .FirstOrDefault();
        }
    }
}
