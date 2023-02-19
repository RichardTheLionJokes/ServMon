using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServMon.Models;

namespace ServMon.Pages.SrvMon.Servers
{
    public class CreateModel : PageModel
    {
        private readonly ServMon.Models.ServMonContext _context;
        public IList<User> Users { get; set; } = default!;

        [BindProperty]
        public Server Server { get; set; } = default!;

        public CreateModel(ServMon.Models.ServMonContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var users = _context.Users.ToList();
            if (users == null)
            {
                return NotFound();
            }
            Users = users;

            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(int[] selectedUsers)
        {
          if (!ModelState.IsValid || _context.Servers == null || Server == null)
            {
                return Page();
            }

            Server.CurrentStatus = ServerStatus.Undefined;

            if (selectedUsers != null)
            {
                foreach (var user in _context.Users.Where(u => selectedUsers.Contains(u.Id)))
                {
                    Server.Users.Add(user);
                }
            }

            _context.Servers.Add(Server);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
