using Microsoft.EntityFrameworkCore;
using nebrangu.Models;

namespace nebrangu.Repositories
{
    public class UsersRepo
    {
        private readonly nebranguContext _context;

        public UsersRepo(nebranguContext context)
        {
            _context = context;
        } 

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users
                .Include(u => u.Name)
                .Include(u => u.Surname)
                .Include(u => u.Email)
                .Include(u => u.Level)
                .Include(u => u.Trustability)
                .Include(u => u.Rating)
                .Include(u => u.Phone)
                .Include(u => u.Address)
                .Include(u => u.City)
                .Include(u => u.PostalCode)
                .Include(u => u.CompanyCode)
                .Include(u => u.BankAccount)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(p => p.Id == id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
