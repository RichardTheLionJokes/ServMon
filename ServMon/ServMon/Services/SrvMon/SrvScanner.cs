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
                                    string address = !string.IsNullOrEmpty(server.Name) ? server.Name : server.IpAddress;
                                    bool online = await Task.Run(() => SrvMethods.AddressIsAvailable(address, 5000));
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
                                            string msg = "<li><b>" + address + "</b> изменил статус на <b>" + newStatus.ToString() + "</b> " + _event.DateTime.ToString() + "</li>";
                                            if (msgs.ContainsKey(user.Email))
                                            {
                                                msgs[user.Email] = msgs[user.Email].Append(msg);
                                            }
                                            else
                                            {
                                                msgs.Add(user.Email, new StringBuilder("<ul>" + msg));
                                            }
                                        }
                                    }
                                }
                            }

                            Parallel.ForEach(msgs, async msg =>
                            {
                                msg.Value.Append("</ul>");
                                List<MailboxAddress> receivers = new List<MailboxAddress>() { new MailboxAddress("", msg.Key) };
                                await Email.SendMail(receivers, "Изменения статусов серверов", msg.Value.ToString());
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // обработка ошибки однократного неуспешного выполнения фоновой задачи
                    Console.WriteLine(ex);
                }

                await Task.Delay(Config.ServStatusCheckFreq*60000, stoppingToken);
            }
        }
    }
}
