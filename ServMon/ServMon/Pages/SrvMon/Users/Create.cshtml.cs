using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServMon.Models;

namespace ServMon.Pages.SrvMon.Users
{
    public class CreateModel : PageModel
    {
        private readonly ServMon.Models.ServMonContext _context;
        public IList<Server> Servers { get; set; } = default!;

        [BindProperty]
        public User User { get; set; } = default!;

        public CreateModel(ServMon.Models.ServMonContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var servers = _context.Servers.ToList();
            if (servers == null)
            {
                return NotFound();
            }
            Servers = servers;

            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Users == null || User == null)
            {
                return Page();
            }

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
