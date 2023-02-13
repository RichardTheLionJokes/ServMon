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
    public class IndexModel : PageModel
    {
        private readonly ServMon.Models.ServMonContext _context;

        public IndexModel(ServMon.Models.ServMonContext context)
        {
            _context = context;
        }

        public IList<ServEvent> ServEvent { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.ServEvents != null)
            {
                ServEvent = await _context.ServEvents.OrderByDescending(e => e.DateTime)
                .Include(s => s.Server).ToListAsync();
            }
        }
    }
}
