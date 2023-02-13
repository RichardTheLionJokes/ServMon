using Microsoft.EntityFrameworkCore;
using MimeKit;
using ServMon.Models;
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
                            var msg = new StringBuilder();

                            IList<Server> Server = await _context.Servers.Where(s => s.Activity).ToListAsync();
                            foreach (Server server in Server)
                            {
                                if (!System.String.IsNullOrEmpty(server.Name))
                                {
                                    bool online = await Task.Run(() => SrvMethods.AddressIsAvailable(server.Name, 5000));
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

                                        msg.Append(server.Name + " изменил статус на " + newStatus.ToString() + " " + _event.DateTime.ToString()).AppendLine();
                                    }
                                }
                            }

                            if (msg.Length > 0 && _context.Users != null)
                            {
                                var users = await _context.Users.Where(u => !System.String.IsNullOrEmpty(u.Email)).ToListAsync();
                                List<MailboxAddress> receivers = new List<MailboxAddress>();
                                foreach (User user in users)
                                {
                                    receivers.Add(new MailboxAddress("", user.Email));
                                }
                                await Email.SendMail(receivers, "Изменения статусов серверов", msg.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // обработка ошибки однократного неуспешного выполнения фоновой задачи
                    Console.WriteLine(ex);
                }

                await Task.Delay(300000, stoppingToken);
            }
        }
    }
}
