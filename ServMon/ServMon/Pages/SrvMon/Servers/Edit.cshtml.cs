using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServMon.Models;

namespace ServMon.Pages.SrvMon.Servers
{
    public class EditModel : PageModel
    {
        private readonly ServMon.Models.ServMonContext _context;

        public EditModel(ServMon.Models.ServMonContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Server Server { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Servers == null)
            {
                return NotFound();
            }

            var server =  await _context.Servers.FirstOrDefaultAsync(m => m.Id == id);
            if (server == null)
            {
                return NotFound();
            }
            Server = server;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Server).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerExists(Server.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ServerExists(int id)
        {
          return (_context.Servers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
