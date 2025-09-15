using CRUD_Application.Models.Entity;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;


namespace CRUD_Application.Services
{
    public class EmailService : IEmailServices
    {

        public async Task SendEmailAsync(string toEmail, string  subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("ydbagora@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart("plain") { Text = message };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync("ydbagora@gmail.com", "cbvx bbqu pnat yvtm");

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}