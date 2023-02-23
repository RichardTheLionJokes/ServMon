using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServMon.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ServMon.Pages.SrvMon.Users
{
    public class EditModel : PageModel
    {
        private readonly ServMon.Models.ServMonContext _context;
        public IList<Server> Servers { get; set; } = default!;
        public string errorMessage = "";

        [BindProperty]
        public User User { get; set; } = default!;

        public EditModel(ServMon.Models.ServMonContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user =  await _context.Users.Include(u => u.Servers).FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            User = user;

            var servers = _context.Servers.ToList();
            if (servers == null)
            {
                return NotFound();
            }
            Servers = servers;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int[] selectedServers)
        {
            if (_context.Servers != null)
            {
                Servers = _context.Servers.ToList();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(User).Collection(u => u.Servers).Load();

            if (string.IsNullOrEmpty(User.Name))
            {
                errorMessage = "Name must be filled!";
                return Page();
            }
            if (string.IsNullOrEmpty(User.Email))
            {
                errorMessage = "Email must be filled!";
                return Page();
            }

            User.Servers.Clear();
            if (selectedServers != null && selectedServers.Length > 0)
            {
                foreach (var serv in _context.Servers.Where(s => selectedServers.Contains(s.Id)))
                {
                    User.Servers.Add(serv);
                }
            }
            _context.Entry(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
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

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
