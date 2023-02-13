using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServMon.Models;

namespace ServMon.Pages.SrvMon.Events
{
    public class DetailsModel : PageModel
    {
        private readonly ServMon.Models.ServMonContext _context;

        public DetailsModel(ServMon.Models.ServMonContext context)
        {
            _context = context;
        }

      public ServEvent ServEvent { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.ServEvents == null)
            {
                return NotFound();
            }

            var servevent = await _context.ServEvents.Include(s => s.Server).FirstOrDefaultAsync(m => m.Id == id);
            if (servevent == null)
            {
                return NotFound();
            }
            else 
            {
                ServEvent = servevent;
            }
            return Page();
        }
    }
}
