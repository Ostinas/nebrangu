using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nebrangu.Models;
using nebrangu.Repositories;
using Newtonsoft.Json.Linq;

namespace nebrangu.Controllers
{
    public class UserController : Controller
    {
        private readonly nebranguContext _context;
        private UsersRepo _repo;
        private EmotionsRepo _repoEmotions;
        private DisputesRepo _disputesRepo;
        private OrdersRepo _ordersRepo;
        private IHttpContextAccessor _httpContextAccessor;

        public UserController(nebranguContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _repo = new UsersRepo(_context);
            _repoEmotions = new EmotionsRepo(_context);
            _ordersRepo = new OrdersRepo(_context);
            _httpContextAccessor = httpContextAccessor;
            _disputesRepo = new DisputesRepo(_context);
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var contr = new DisputeController(_context, _httpContextAccessor, _disputesRepo, _repo);
            var result = await contr.TrustCalculation(2);
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

        public async Task<IActionResult> OpenProfilePage()
        {
            User user = await _repo.GetUserById(1);
            return View("ProfilePage", user);
        }

        public async Task<IActionResult> OpenOrderHistory()
        {
            OrderController orderController = new OrderController(_context, _httpContextAccessor);

            var orders = await orderController.GetOrderList(1);
            return View("OrderHistoryPage", orders);
        }

        public bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public string GetSellerBankAccount(int userId)
        {
            return _repo.GetBankAccount(userId);
        }

        public async Task<List<Product>> CalculateEmotionCoefficientAsync()
        {
            List<Product> RecommendedProducts = new List<Product>();

            
            double emotionCoefficient = 0;

            string emotion = _httpContextAccessor.HttpContext.Request.Cookies["Emotion"];

            if (emotion == null)
            {
                emotion = "3";
            }
            else
            {

                if (emotion == "1")
                    emotionCoefficient = 1.5;
                else if (emotion == "2")
                    emotionCoefficient = 1.2;
                else if (emotion == "3")
                    emotionCoefficient = 1;
                else if (emotion == "4")
                    emotionCoefficient = 0.8;
                else if (emotion == "5")
                    emotionCoefficient = 0.5;
            }


            var orderController = new OrderController(_context, _httpContextAccessor);
            var productController = new ProductController(_context, _httpContextAccessor);

            if (orderController != null && productController != null)
            {

                List<int> category = await orderController.CheckOrderHistory();

                List<string> SeasonAndWeather = await GetWeatherData();

                List<Product> Products = await productController.GetAllProducts();

                RecommendedProducts = productController.FilterBySeasonAndWeather(category, SeasonAndWeather, Products, emotionCoefficient);
            }


            return RecommendedProducts;

        }




        private async Task<List<string>> GetWeatherData()
        {
            List<string> list = new List<string>();
            HttpClient client = new HttpClient();
            string url = "https://api.meteo.lt/v1/stations/vilniaus-ams/observations/latest";

            HttpResponseMessage response = await client.GetAsync(url);

            if (response != null && response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(content);

                var observations = json["observations"][0];

                string season = GetSeason(observations["observationTimeUtc"].Value<string>());
                string weather = GetWeather(observations["conditionCode"].Value<string>());

                list.Add(season);
                list.Add(weather);

                return list;
            }
            else
            {
                list.Add("API nuskaitymo klaida.");
                return list;
            }
        }

        static string GetSeason(string dateString)
        {
            DateTime observationTime = DateTime.Parse(dateString);

            int month = observationTime.Month;
            int day = observationTime.Day;

            if (month == 12 || month == 1 || month == 2)
            {
                return "Ziema";
            }
            else if (month == 3 || month == 4 || month == 5)
            {
                return "Pavasaris";
            }
            else if (month == 6 || month == 7 || month == 8)
            {
                return "Vasara";
            }
            else
            {
                return "Ruduo";
            }
        }

        static string GetWeather(string weatherCode)
        {
            switch (weatherCode)
            {
                case "clear":
                    return "giedra";
                case "partly-cloudy":
                    return "mažai debesuota";
                case "variable-cloudiness":
                    return "nepastoviai debesuota";
                case "cloudy-with-sunny-intervals":
                    return "debesuota su pragiedruliais";
                case "cloudy":
                    return "debesuota";
                case "thunder":
                    return "perkūnija";
                case "isolated-thunderstorms":
                    return "trumpas lietus su perkūnija";
                case "thunderstorms":
                    return "lietus su perkūnija";
                case "light-rain":
                    return "nedidelis lietus";
                case "rain":
                    return "lietus";
                case "heavy-rain":
                    return "smarkus lietus";
                case "rain-showers":
                    return "trumpas lietus";
                case "light-rain-at-times":
                    return "protarpiais nedidelis lietus";
                case "rain-at-times":
                    return "protarpiais lietus";
                case "light-sleet":
                    return "nedidelė šlapdriba";
                case "sleet":
                    return "šlapdriba";
                case "sleet-at-times":
                    return "protarpiais šlapdriba";
                case "sleet-showers":
                    return "trumpa šlapdriba";
                case "freezing-rain":
                    return "lijundra";
                case "hail":
                    return "kruša";
                case "light-snow":
                    return "nedidelis sniegas";
                case "snow":
                    return "sniegas";
                case "heavy-snow":
                    return "smarkus sniegas";
                case "snow-showers":
                    return "trumpas sniegas";
                case "snow-at-times":
                    return "protarpiais sniegas";
                case "light-snow-at-times":
                    return "protarpiais nedidelis sniegas";
                case "snowstorm":
                    return "pūga";
                case "mist":
                    return "rūkana";
                case "fog":
                    return "rūkas";
                case "squall":
                    return "škvalas";
                default:
                    return "Visi";
            }

        }

        public async Task<double> RequestTrustability(int userId)
        {
           var user = await _repo.GetUserById(userId);
           return user.Trustability;
        }

        public async Task<int> GetOrderCount(int userId)
        {
            var orderList = await _ordersRepo.GetUserOrders(userId);
            int orderCount = orderList.Count;
            return orderCount;
        }

        public async Task<User> GetUserInfo(int userId)
        {
            var user = await _repo.GetUserById(userId);
            return user;
        }

    }
}
