﻿using System;
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
    public class DisputeController : Controller
    {
        private readonly nebranguContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        private DisputesRepo _repo;
        private UsersRepo _usersRepo;

        public DisputeController(nebranguContext context, IHttpContextAccessor httpContextAccessor, DisputesRepo disputesRepo, UsersRepo usersRepo)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _repo = disputesRepo;
            _usersRepo = usersRepo;
        }

        // GET: Dispute
        public async Task<IActionResult> Index()
        {
              return _context.Dispute != null ? 
                          View(await _context.Dispute.ToListAsync()) :
                          Problem("Entity set 'nebranguContext.Dispute'  is null.");
        }

        // GET: Dispute/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Dispute == null)
            {
                return NotFound();
            }

            var dispute = await _context.Dispute
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dispute == null)
            {
                return NotFound();
            }

            return View(dispute);
        }

        // GET: Dispute/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dispute/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SolutionScore")] Dispute dispute)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dispute);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dispute);
        }

        // GET: Dispute/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Dispute == null)
            {
                return NotFound();
            }

            var dispute = await _context.Dispute.FindAsync(id);
            if (dispute == null)
            {
                return NotFound();
            }
            return View(dispute);
        }

        // POST: Dispute/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SolutionScore")] Dispute dispute)
        {
            if (id != dispute.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dispute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisputeExists(dispute.Id))
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
            return View(dispute);
        }

        // GET: Dispute/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Dispute == null)
            {
                return NotFound();
            }

            var dispute = await _context.Dispute
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dispute == null)
            {
                return NotFound();
            }

            return View(dispute);
        }

        // POST: Dispute/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Dispute == null)
            {
                return Problem("Entity set 'nebranguContext.Dispute'  is null.");
            }
            var dispute = await _context.Dispute.FindAsync(id);
            if (dispute != null)
            {
                _context.Dispute.Remove(dispute);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<double> TrustCalculation(int userId = 2)
        {
            var userController = new UserController(_context, _httpContextAccessor);
            var userTrustability = await userController.RequestTrustability(userId);
            var tasks = new List<Task>();
            double coefficient = 1;
            double coefficient2 = 1;
            double coefficient3 = 1;
            double coefficient4 = 1;

            var userInfo = await userController.GetUserInfo(userId);
            tasks.Add(Task.Run(async () =>
            {
                var orderCount = await userController.GetOrderCount(userId);

                if (orderCount > 30)
                {
                    coefficient = ChangeCoefficient(coefficient, 1.25);
                }
                else if (orderCount > 10 && orderCount < 30)
                {
                    coefficient = ChangeCoefficient(coefficient, 1.1);
                }
                else if (orderCount < 10 && orderCount > 1)
                {
                    coefficient = ChangeCoefficient(coefficient, 1.05);
                }

                var lostDisputes = await GetLostDisputes(userId);
                if (lostDisputes > 1)
                {
                    var coef = ((1 / lostDisputes) + coefficient) / 2;
                    coefficient = ChangeCoefficient(coefficient, coef);
                }
            }));


            tasks.Add(Task.Run(async () =>
            {
               
                if (userInfo.CompanyCode != null)
                {
                    if (userInfo.CompanyIncome > 5000)
                    {
                        coefficient2 = ChangeCoefficient(coefficient2, 1.2);
                    }
                    else
                    {
                        coefficient2 = ChangeCoefficient(coefficient3, 0.9);
                    }
                }
            }));

            tasks.Add(Task.Run(async () =>
            {
                var userRating = userInfo.Rating;
                if (userRating > 4)
                {
                    coefficient3 = ChangeCoefficient(coefficient3, 1.2);
                }
                else if (userRating > 3 && userRating < 4)
                {
                    coefficient3 = ChangeCoefficient(coefficient3, 1.1);
                }
                else
                {
                    coefficient3 = ChangeCoefficient(coefficient3, 0.8);
                }
            }));

            tasks.Add(Task.Run(async () =>
            {
                var userDate = userInfo.RegistrationDate;
                TimeSpan difference = DateTime.Now - userDate;
                if (difference >= TimeSpan.FromDays(30))
                {
                    coefficient4 = ChangeCoefficient(coefficient4, 1.1);
                }
            }));

            await Task.WhenAll(tasks);

            var averageCoeff = AverageCoefficient(coefficient, coefficient2, coefficient3, coefficient4);

            ChangeTrustability(averageCoeff, userId);

            return averageCoeff;


        }

        public async Task<int> GetLostDisputes(int userId)
        {

            var sellerLostDisputes = await _repo.GetBySellerId(userId);
            return sellerLostDisputes;
        }

        public double ChangeCoefficient(double coefficient, double newCoefficient) 
        {
            return coefficient = newCoefficient;
        }

        public double AverageCoefficient(double coefficient, double coefficient2, double coefficient3, double coefficient4) 
        {
            return (coefficient + coefficient2 + coefficient3 + coefficient4) / 4;
        }

        public async void ChangeTrustability(double coefficient, int userId)
        {
            var user = await _usersRepo.GetById(userId);
            user.Trustability *= coefficient;
            await _usersRepo.Update(user);
        }

        public bool DisputeExists(int id)
        {
          return (_context.Dispute?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
