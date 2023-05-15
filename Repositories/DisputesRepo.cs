using Microsoft.EntityFrameworkCore;
using nebrangu.Models;

namespace nebrangu.Repositories
{
    public class DisputesRepo
    {
        private readonly nebranguContext _context;

        public DisputesRepo(nebranguContext context)
        {
            _context = context;
        }

        public async Task<List<Dispute>> GetAll()
        {
            return await _context.Dispute
                .Include(s => s.Solution)
                .Include(s => s.Problem)
                .Include(s => s.Buyer)
                .Include(s => s.Seller)
                .ToListAsync();
        }

        public async Task<Dispute> GetById(int id)
        {
            return await _context.Dispute
                .Include(s => s.Solution)
                .Include(s => s.Problem)
                .Include(s => s.Buyer)
                .Include(s => s.Seller)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<int> GetBySellerId(int userId)
        {
            return await _context.Dispute
                .Include(s => s.Solution)
                .Include(s => s.Problem)
                .Include(s => s.Buyer)
                .Include(s => s.Seller)
                .Where(s => s.Seller.Id == userId)
                .Where(s => s.Solution.Name == "Pirkėjas teisus")
                .CountAsync();
        }

        public async Task<Dispute> Create(Dispute dispute)
        {
            _context.Dispute.Add(dispute);
            await _context.SaveChangesAsync();
            return dispute;
        }

        public async Task<Dispute> Update(Dispute dispute)
        {
            _context.Dispute.Update(dispute);
            await _context.SaveChangesAsync();
            return dispute;
        }

        public async Task<Dispute> Delete(int id)
        {
            var dispute = await _context.Dispute.FirstOrDefaultAsync(p => p.Id == id);
            _context.Dispute.Remove(dispute);
            await _context.SaveChangesAsync();
            return dispute;
        }

    }
}
