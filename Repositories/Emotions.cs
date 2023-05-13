using Microsoft.EntityFrameworkCore;
using nebrangu.Models;

namespace nebrangu.Repositories
{
    public class EmotionsRepo
    {
        private readonly nebranguContext _context;

        public EmotionsRepo(nebranguContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Mood>> GetAll()
        {
            return _context.Moods.AsEnumerable();
        }
    }
}
