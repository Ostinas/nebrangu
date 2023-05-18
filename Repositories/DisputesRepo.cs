using Google.Protobuf.WellKnownTypes;
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
            var entry = await _context.Dispute.AddAsync(dispute);
            await _context.SaveChangesAsync();

            return entry.Entity;
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

        public async void AddToScore(double value, int id)
        {
            var dispute = await _context.Dispute.FirstOrDefaultAsync(p => p.Id == id);

            dispute.SolutionScore += value;

            _context.Dispute.Update(dispute);
            await _context.SaveChangesAsync();

        }

        public async void SideWithBuyer(int id)
        {
            var dispute = await _context.Dispute
                .Include(o => o.Solution)
                .FirstOrDefaultAsync(p => p.Id == id);



            dispute.SolutionId = 1;

            dispute.Solution = new Solution() { Id = 1, Name = "Pirkėjas teisus" };

            _context.Dispute.Update(dispute);
            await _context.SaveChangesAsync();
        }

        public async void SideWithSeller(int id)
        {
            var dispute = await _context.Dispute
               .Include(o => o.Solution)
               .FirstOrDefaultAsync(p => p.Id == id);



            dispute.SolutionId = 2;
            dispute.Solution = new Solution() { Id = 2, Name = "Pardavėjas teisus" };


            _context.Dispute.Update(dispute);
            await _context.SaveChangesAsync();
        }

    }
}
