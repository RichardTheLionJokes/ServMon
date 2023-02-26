using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServMon.Models;
using ServMon.Services;

namespace ServMon.Pages.SrvMon.Users
{
    public class IndexModel : PageModel
    {
        private readonly ServMon.Models.ServMonContext _context;
        public PaginatedList<User>? userPList { get; set; } = default!;

        public IndexModel(ServMon.Models.ServMonContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync(int? pageIndex, int? pageSize)
        {
            IQueryable<User> User;

            if (_context.Users != null)
            {
                User = from u in _context.Users
                         select u;

                try
                {
                    userPList = await PaginatedList<User>.CreateAsync(User, pageIndex ?? 1, pageSize ?? 10);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
