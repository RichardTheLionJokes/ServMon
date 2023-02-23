using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using ServMon.Models;
using ServMon.Services;

namespace ServMon.Pages.SrvMon.Events
{
    public class IndexModel : PageModel
    {
        private readonly ServMon.Models.ServMonContext _context;
        public PaginatedList<ServEvent>? servEventPList { get; set; } = default!;

        public IndexModel(ServMon.Models.ServMonContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync(int? pageIndex, int? pageSize)
        {
            IQueryable<ServEvent> ServEvent;

            if (_context.ServEvents != null)
            {
                ServEvent = _context.ServEvents.OrderByDescending(e => e.DateTime)
                .Include(s => s.Server);

                try
                {
                    servEventPList = await PaginatedList<ServEvent>.CreateAsync(ServEvent, pageIndex ?? 1, pageSize ?? 10);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
