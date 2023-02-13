using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MimeKit;
using NuGet.Packaging;
using static System.Net.Mime.MediaTypeNames;

namespace ServMon.Services
{
    public static class Email
    {
        public static async Task SendMail(List<MailboxAddress> _receivers, string _subject, string _message)
        {
            try
            {
                using (var smtp = new SmtpClient())
                {
                    await smtp.ConnectAsync(Config.YandexHost, Config.YandexPort, Config.YandexSsl);
                    await smtp.AuthenticateAsync(Config.YandexLogin, Config.YandexPass);

                    var msg = new MimeMessage()
                    {
                        Subject = _subject,
                        Body = new TextPart(MimeKit.Text.TextFormat.Html)
                        {
                            Text = _message
                        }
                    };

                    msg.To.AddRange(_receivers);
                    msg.From.Add(new MailboxAddress("AmGPGU-IT", Config.YandexLogin));
                    
                    await smtp.SendAsync(msg);
                    await smtp.DisconnectAsync(true);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
