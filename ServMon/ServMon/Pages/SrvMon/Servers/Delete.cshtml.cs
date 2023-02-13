using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServMon.Models;

namespace ServMon.Pages.SrvMon.Servers
{
    public class DeleteModel : PageModel
    {
        private readonly ServMon.Models.ServMonContext _context;

        public DeleteModel(ServMon.Models.ServMonContext context)
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

            var server = await _context.Servers.FirstOrDefaultAsync(m => m.Id == id);

            if (server == null)
            {
                return NotFound();
            }
            else 
            {
                Server = server;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Servers == null)
            {
                return NotFound();
            }
            var server = await _context.Servers.FindAsync(id);

            if (server != null)
            {
                Server = server;
                _context.Servers.Remove(Server);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
