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
        public string errorMessage = "";

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
        public async Task<IActionResult> OnPostAsync(int[] selectedServers)
        {

            if (_context.Servers != null)
            {
                Servers = _context.Servers.ToList();
            }

            if (!ModelState.IsValid || _context.Users == null || User == null)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(User.Name) || string.IsNullOrEmpty(User.Email))
            {
                if (string.IsNullOrEmpty(User.Name))
                {
                    errorMessage = "Name must be filled!";
                }
                if (string.IsNullOrEmpty(User.Email))
                {
                    errorMessage = (string.IsNullOrEmpty(errorMessage) ? "" : errorMessage + " ") + "Email must be filled!";
                }
                return Page();
            }

            if (selectedServers != null && selectedServers.Length > 0)
            {
                foreach (var serv in _context.Servers.Where(s => selectedServers.Contains(s.Id)))
                {
                    User.Servers.Add(serv);
                }
            }

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
