using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace VegankoService.Services
{
    public class EmailService : IEmailService
    {
        public static readonly List<EmailProvider> KnownEmailProviders = new List<EmailProvider>
        {
            new EmailProvider("gmail.com", "smtp.gmail.com", 587),
            new EmailProvider("outlook.com", "smtp.live.com", 587),
            new EmailProvider("yahoo.com", "smtp.mail.yahoo.com", 465),
            new EmailProvider("hotmail.com", "smtp.live.com", 465),
            new EmailProvider("office365.com", "smtp.office365.com", 587),
            new EmailProvider("gmx.com", "smtp.gmx.com", 465),
            new EmailProvider("mail.com", "smtp.mail.com", 587),
        };

        private readonly IConfiguration configuration;
        private readonly ILogger<EmailService> logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public bool IsEmailProviderSupported(string email)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(email);
                return KnownEmailProviders.Any(emp => emp.Host == mailAddress.Host);
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Invalid email format.");
            }

            return false;
        }

        public Task SendEmail(string email, string subject, string message)
        {
            return SendEmail(new MailAddress(email), subject, message);
        }

        public async Task SendEmail(MailAddress receiverEmail, string subject, string message)
        {
            MailAddress senderEmail = new MailAddress(configuration.GetSection("EmailService")["Email"]);
            string senderPassword = configuration.GetSection("EmailService")["Password"];

            var mimeMsg = new MimeMessage();
            mimeMsg.From.Add(new MailboxAddress
                ("Veganko",
                senderEmail.Address));
            mimeMsg.To.Add(new MailboxAddress
                ("Receiver",
                receiverEmail.Address));
            mimeMsg.Subject = subject;
            mimeMsg.Body = new TextPart("html")
            {
                Text = message
            };

            logger.LogDebug("Receiver email provider: " + receiverEmail.Host);


            EmailProvider? emProvider = KnownEmailProviders
                .Select(emp => new EmailProvider?(emp))
                .FirstOrDefault(emp => emp.Value.Host == receiverEmail.Host);

            if (emProvider == null)
            {
                throw new ArgumentException($"Unsupported email provider: {receiverEmail.Host}.");
            }

            using (var client = new SmtpClient())
            {
                client.Connect(emProvider.Value.Url, emProvider.Value.Port, emProvider.Value.SSL);
                client.Authenticate(
                    senderEmail.Address,
                    senderPassword);

                await client.SendAsync(mimeMsg);
                logger.LogDebug("The mail has been sent successfully !");

                await client.DisconnectAsync(true);
            }
        }

        public struct EmailProvider
        {
            public EmailProvider(string host, string url, int port, bool ssl = false)
            {
                Host = host;
                Url = url;
                Port = port;
                SSL = ssl;
            }

            public string Host { get; set; }
            public string Url { get; set; }
            public int Port { get; set; }
            public bool SSL { get; set; }
        }
    }
}
