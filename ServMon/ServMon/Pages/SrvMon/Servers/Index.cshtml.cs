using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using ServMon.Models;
using ServMon.Pages.SrvMon.Events;
using ServMon.Services;
using ServMon.Services.SrvMon;

namespace ServMon.Pages.SrvMon.Servers
{
    public class IndexModel : PageModel
    {
        private readonly ServMon.Models.ServMonContext _context;

        public IndexModel(ServMon.Models.ServMonContext context)
        {
            _context = context;
        }

        public IList<Server> Server { get;set; } = default!;

        public async Task OnGetAsync(int? id)
        {
            if (_context.Servers == null) return;
            else Server = await _context.Servers.ToListAsync();

            if (id != null)
            {
                var server = await _context.Servers.FirstOrDefaultAsync(m => m.Id == id);
                if (server == null)
                {
                    return;
                }
                else
                {
                    if (!System.String.IsNullOrEmpty(server.Name))
                    {
                        bool online = SrvMethods.AddressIsAvailable(server.Name, 1000);
                        ServerStatus newStatus = online ? ServerStatus.Available : ServerStatus.NotAvailable;
                        if (server.CurrentStatus != newStatus)
                        {
                            server.CurrentStatus = newStatus;
                            ServEvent _event = new ServEvent();
                            _event.ServerId = server.Id;
                            _event.DateTime = DateTime.Now;
                            _event.Type = ServEventType.StatusChanged;
                            _event.ServerStatus = newStatus;
                            _context.ServEvents.Add(_event);

                            await _context.SaveChangesAsync();

                            string msg = server.Name + " изменил статус на " + newStatus.ToString() + " " + _event.DateTime.ToString();
                            if (_context.Users != null)
                            {
                                var users = await _context.Users.Where(u => !System.String.IsNullOrEmpty(u.Email)).ToListAsync();
                                List<MailboxAddress> receivers = new List<MailboxAddress>();
                                foreach (User user in users)
                                {
                                    receivers.Add(new MailboxAddress("", user.Email));
                                }
                                await Email.SendMail(receivers, "Изменения статусов серверов", msg);
                            }
                        }
                    }
                }

                RedirectToPage("./Index");
            }
        }
    }
}
