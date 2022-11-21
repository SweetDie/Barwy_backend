﻿using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Barwy.Services
{
    public class EmailService
    {
        private IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            string fromEmail = _configuration["EmailSettings:User"];
            string SMTP = _configuration["EmailSettings:SMTP"];
            int PORT = int.Parse(_configuration["EmailSettings:PORT"]);
            string password = _configuration["EmailSettings:Password"];

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(fromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            email.Body = bodyBuilder.ToMessageBody();

            // send email
            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.Connect(SMTP, PORT, SecureSocketOptions.SslOnConnect);
                    smtp.Authenticate(fromEmail, password);
                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
