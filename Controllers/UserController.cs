using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nebrangu.Models;
using nebrangu.Repositories;
using Newtonsoft.Json;

namespace nebrangu.Controllers
{
    public class UserController : Controller
    {
        private readonly nebranguContext _context;
        private UsersRepo _repo;
        private EmotionsRepo _repoEmotions;

        public UserController(nebranguContext context)
        {
            _context = context;
            _repo = new UsersRepo(_context);
            _repoEmotions = new EmotionsRepo(_context);
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _repo.GetAll();
            return View("UserListPage", users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _repo.GetById((int)id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Email,Level,Trustability,Rating,Phone,Address,City,PostalCode,CompanyCode,BankAccount")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Email,Level,Trustability,Rating,Phone,Address,City,PostalCode,CompanyCode,BankAccount")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'nebranguContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EmotionSelection()
        {
            var emotions = await _repoEmotions.GetAll();
            return View("EmotionSelectionPage", emotions);
        }

        public async Task<IActionResult> SelectEmotion(int emotion)
        {
            HttpContext.Response.Cookies.Append("Emotion", emotion.ToString(), new CookieOptions()
            {
                Expires = DateTime.Now.AddHours(24)
            });
            return RedirectToAction("Index", "Home");
        }

        public bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public string GetSellerBankAccount(int userId)
        {
            return _repo.GetBankAccount(userId);
        }

        public async Task<IActionResult> Profilis()
        {
            //string cartCookieJson = _httpContextAccessor.HttpContext.Request.Cookies["cart"];

            //Dictionary<int, int> cart = !string.IsNullOrEmpty(cartCookieJson) ? JsonConvert.DeserializeObject<Dictionary<int, int>>(cartCookieJson) : new Dictionary<int, int>();

            //var products = from p in _context.Products
                          // where cart.Keys.Contains(p.Id)
                          // select p;

            return View("ProfilePage");
        }
    }
}
