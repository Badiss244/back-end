using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Domain.Interface;
using Microsoft.Extensions.Configuration;

namespace Domain.MailSender
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
     
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]);
            var smtpUser = _configuration["Smtp:User"];
            var smtpPass = _configuration["Smtp:Pass"];
            var fromEmail = _configuration["Smtp:FromEmail"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true; 
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
