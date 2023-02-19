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

            // проверка статуса сервера
            if (id != null)
            {
                var server = await _context.Servers.Include(s => s.Users).FirstOrDefaultAsync(m => m.Id == id);
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
                            _event.Server = server;
                            _event.DateTime = DateTime.Now;
                            _event.Type = ServEventType.StatusChanged;
                            _event.ServerStatus = newStatus;
                            _context.ServEvents.Add(_event);

                            await _context.SaveChangesAsync();

                            if (server.Users.Count > 0)
                            {
                                string msg = "<b>" + server.Name + "</b> изменил статус на <b>" + newStatus.ToString() + "</b> " + _event.DateTime.ToString();
                                List<MailboxAddress> receivers = new List<MailboxAddress>();
                                foreach (User user in server.Users)
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
            // конец проверки статуса сервера
        }
    }
}
