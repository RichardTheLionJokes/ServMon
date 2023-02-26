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
using ServMon.Pages.SrvMon.Events;
using ServMon.Services;
using ServMon.Services.SrvMon;

namespace ServMon.Pages.SrvMon.Servers
{
    public class IndexModel : PageModel
    {
        private readonly ServMon.Models.ServMonContext _context;
        public PaginatedList<Server>? serverPList { get; set; } = default!;

        public IndexModel(ServMon.Models.ServMonContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync(int? pageIndex, int? pageSize, int? id)
        {
            IQueryable<Server> Server;

            if (_context.Servers != null)
            {
                Server = from s in _context.Servers
                         select s;

                try
                {
                    serverPList = await PaginatedList<Server>.CreateAsync(Server, pageIndex ?? 1, pageSize ?? 10);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            // проверка статуса сервера
            if (id != null && _context.Servers != null && _context.Users != null)
            {
                var server = await _context.Servers.Include(s => s.Users).FirstOrDefaultAsync(m => m.Id == id);
                if (server == null)
                {
                    return;
                }
                else
                {
                    if (!string.IsNullOrEmpty(server.Name) || !string.IsNullOrEmpty(server.IpAddress))
                    {
                        string address = !string.IsNullOrEmpty(server.Name) ? server.Name : server.IpAddress;
                        bool online = SrvMethods.AddressIsAvailable(address, 1000);
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
                                string msg = "<b>" + address + "</b> изменил статус на <b>" + newStatus.ToString() + "</b> " + _event.DateTime.ToString();
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
            }
            // конец проверки статуса сервера
        }
    }
}
