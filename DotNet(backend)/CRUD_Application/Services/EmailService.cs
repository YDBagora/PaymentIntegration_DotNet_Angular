using CRUD_Application.Models.Entity;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CRUD_Application.Services
{
    public class EmailService : IEmailServices
    {
        private readonly EmailSettings _emailServices;

        public EmailService(IOptions<EmailSettings> emailServices)
        {
            _emailServices = emailServices.Value;
        }

        public async Task SendEmailAsync(string toEmail, string  subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailServices.SenderEmailId));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailServices.SmtpServer, _emailServices.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailServices.UserName, _emailServices.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
