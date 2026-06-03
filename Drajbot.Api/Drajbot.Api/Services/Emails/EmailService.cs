using Drajbot.Api.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Drajbot.Api.Services.Emails
{
    public class EmailService(IConfiguration config) : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = config["EmailSettings:Host"]!;
            var smtpPort = int.Parse(config["EmailSettings:Port"]!);
            var smtpUser = config["EmailSettings:User"]!;
            var smtpPass = config["EmailSettings:Password"]!;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true // Bezbedna konekcija
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser, "D'RAJBOT Game Shop"), // Ovo će pisati klijentu!
                Subject = subject,
                Body = body,
                IsBodyHtml = true // HTML format da bi mejl izgledao lepo
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}