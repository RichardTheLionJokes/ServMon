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
        public IList<User> Users { get; set; } = default!;

        [BindProperty]
        public Server Server { get; set; } = default!;

        public EditModel(ServMon.Models.ServMonContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Servers == null)
            {
                return NotFound();
            }

            var server = await _context.Servers.Include(s => s.Users).FirstOrDefaultAsync(m => m.Id == id);
            if (server == null)
            {
                return NotFound();
            }
            Server = server;

            var users = _context.Users.ToList();
            if (users == null)
            {
                return NotFound();
            }
            Users = users;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int[] selectedUsers)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //_context.Attach(Server).State = EntityState.Modified;
            _context.Attach(Server).Collection(u => u.Users).Load();
            Server.Users.Clear();
            if (selectedUsers != null)
            {
                foreach (var user in _context.Users.Where(u => selectedUsers.Contains(u.Id)))
                {
                    Server.Users.Add(user);
                }
            }
            _context.Entry(Server).State = EntityState.Modified;

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
