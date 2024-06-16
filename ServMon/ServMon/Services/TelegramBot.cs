using Microsoft.EntityFrameworkCore;
using ServMon.Models;
using ServMon.Services.SrvMon;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.Metrics;

namespace ServMon.Services
{
    public class TelegramBot : BackgroundService
    {
        static string? connection = Config.ConnectionString;
        static DbContextOptionsBuilder<ServMonContext> optionsBuilder = new DbContextOptionsBuilder<ServMonContext>();
        static DbContextOptions<ServMonContext> options = optionsBuilder.UseSqlServer(connection).Options;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var botClient = new TelegramBotClient(Config.TelegramBotToken);
                botClient.StartReceiving(Update, Error);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                Console.WriteLine($"{message.Chat.FirstName} | {message.Text}");
                if (message.Text.ToLower().StartsWith("/help"))
                {
                    string resp = "Список доступных команд:" + "\n"
                        + "/srvcheck список имен или ip-адресов через пробел - проверка доступности всех серверов" + "\n"
                        + "/srvcheck all - проверка доступности всех серверов" + "\n"
                        + "/allsrvs - список всех серверов";
                    await botClient.SendTextMessageAsync(message.Chat.Id, resp);
                    return;
                }
                else if (message.Text.ToLower().StartsWith("/srvcheck"))
                {
                    StringBuilder resp = new StringBuilder();
                    string[] addresses = message.Text.Substring(9).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (addresses.Length > 0 && addresses[0].ToLower() == "all")
                    {
                        using (ServMonContext _context = new ServMonContext(options))
                        {
                            if (_context.Servers != null)
                            {
                                IEnumerable<Server> Servers = _context.Servers.Include(s => s.Users).Where(s => s.Activity);
                                addresses = new string[Servers.Count()];
                                int i = 0;
                                foreach (Server server in Servers)
                                {
                                    addresses[i] = !string.IsNullOrEmpty(server.Name) ? server.Name : server.IpAddress;
                                    i++;
                                }
                            }
                        }
                    }
                    foreach (string address in addresses)
                    {
                        bool online = await Task.Run(() => SrvMethods.AddressIsAvailable(address, 5000));
                        if (resp.Length != 0) resp.AppendLine();
                        resp.Append($"{address} {(online ? "On" : "Off")}");
                    }
                    if (resp.Length == 0) resp.Append("Укажите серверы для проверки доступности");
                    await botClient.SendTextMessageAsync(message.Chat.Id, resp.ToString());
                    return;
                }
                else if (message.Text.ToLower().StartsWith("/allsrvs"))
                {
                    StringBuilder resp = new StringBuilder();
                    try
                    {
                        using (ServMonContext _context = new ServMonContext(options))
                        {
                            if (_context.Servers != null)
                            {
                                IEnumerable<Server> Servers = _context.Servers.Include(s => s.Users).Where(s => s.Activity);
                                foreach (Server server in Servers)
                                {
                                    if (resp.Length != 0) resp.AppendLine();
                                    resp.Append($"name: {server.Name} | address: {server.IpAddress} | description: {server.ShortDescription}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, resp.ToString());
                    return;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
                    return;
                }
            }
        }

        private async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}