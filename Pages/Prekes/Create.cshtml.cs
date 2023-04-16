using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using nebrangu.Data;
using nebrangu.Models;

namespace nebrangu.Pages.Prekes
{
    public class CreateModel : PageModel
    {
        private readonly nebrangu.Data.nebranguContext _context;

        public CreateModel(nebrangu.Data.nebranguContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Name");
        ViewData["ManufacturerId"] = new SelectList(_context.Set<Manufacturer>(), "Id", "Name");
        ViewData["SeasonId"] = new SelectList(_context.Set<Season>(), "Id", "Name");
        ViewData["WeatherId"] = new SelectList(_context.Set<Weather>(), "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Products == null || Product == null)
            {
                return Page();
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
