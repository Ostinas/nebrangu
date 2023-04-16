using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using nebrangu.Data;
using nebrangu.Models;

namespace nebrangu.Pages.Prekes
{
    public class IndexModel : PageModel
    {
        private readonly nebrangu.Data.nebranguContext _context;

        public IndexModel(nebrangu.Data.nebranguContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Products != null)
            {
                Product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Season)
                .Include(p => p.Weather).ToListAsync();
            }
        }
    }
}
