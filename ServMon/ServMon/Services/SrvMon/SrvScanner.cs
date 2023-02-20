using Microsoft.EntityFrameworkCore;
using MimeKit;
using ServMon.Models;
using ServMon.Pages.SrvMon.Users;
using System.Text;

namespace ServMon.Services.SrvMon
{
    public class SrvScanner : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string? connection = Config.ConnectionString;
                    var optionsBuilder = new DbContextOptionsBuilder<ServMonContext>();
                    var options = optionsBuilder.UseSqlServer(connection).Options;
                    using (ServMonContext _context = new ServMonContext(options))
                    {
                        if (_context.Servers != null)
                        {
                            Dictionary<string, StringBuilder> msgs = new Dictionary<string, StringBuilder>();

                            IList<Server> Servers = await _context.Servers.Include(s => s.Users).Where(s => s.Activity).ToListAsync();
                            foreach (Server server in Servers)
                            {
                                if (!string.IsNullOrEmpty(server.Name) || !string.IsNullOrEmpty(server.IpAddress))
                                {
                                    bool online = await Task.Run(() => SrvMethods.AddressIsAvailable(server.Name, server.IpAddress, 5000));
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

                                        foreach (User user in server.Users)
                                        {
                                            if (msgs.ContainsKey(user.Email))
                                            {
                                                msgs[user.Email] = msgs[user.Email].Append("<li><b>" + server.Name + "</b> изменил статус на <b>" + newStatus.ToString() + "</b> " + _event.DateTime.ToString() + "</li>");
                                            }
                                            else
                                            {
                                                var msg = new StringBuilder("<ul><li><b>" + server.Name + "</b> изменил статус на <b>" + newStatus.ToString() + "</b> " + _event.DateTime.ToString() + "</li>");
                                                msgs.Add(user.Email, msg);
                                            }
                                        }
                                    }
                                }
                            }

                            foreach (var msg in msgs)
                            {
                                msg.Value.Append("</ul>");
                                List<MailboxAddress> receivers = new List<MailboxAddress>() { new MailboxAddress("", msg.Key) };
                                await Email.SendMail(receivers, "Изменения статусов серверов", msg.Value.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // обработка ошибки однократного неуспешного выполнения фоновой задачи
                    Console.WriteLine(ex);
                }

                await Task.Delay(5*60000, stoppingToken);
            }
        }
    }
}
