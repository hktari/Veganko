﻿using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Services
{
    public class EmailService : IEmailService
    {
        public EmailService()
        {

        }
        private readonly Dictionary<string, EmailProvider> knownEmailProviders = new Dictionary<string, EmailProvider>
        {
            { "gmail", new EmailProvider("gmail", "smtp.gmail.com", 587) },
            { "outlook", new EmailProvider("outlook", "smtp.live.com", 587) },
            { "yahoo", new EmailProvider("yahoo", "smtp.mail.yahoo.com", 465) },
            { "hotmail", new EmailProvider("hotmail", "smtp.live.com", 465) },
            { "office365", new EmailProvider("office365", "smtp.office365.com", 587) },
        };

        public async Task SendEmail(string email, string subject, string message)
        {
            var mimeMsg = new MimeMessage();
            mimeMsg.From.Add(new MailboxAddress
                ("Veganko",
                "vegankoapp@gmail.com"));
            mimeMsg.To.Add(new MailboxAddress
                ("Receiver",
                email));
            mimeMsg.Subject = subject;
            mimeMsg.Body = new TextPart("html")
            {
                Text = message
            };

            int atSignIdx = email.IndexOf("@");
            string targetEmailProvider = email.Substring(
                atSignIdx + 1, email.LastIndexOf('.') - atSignIdx - 1);
            Console.WriteLine("target email pvovider: " + targetEmailProvider);

            EmailProvider emProvider = knownEmailProviders[targetEmailProvider];

            using (var client = new SmtpClient())
            {
                // TODO: get hostname port and ssl
                client.Connect(emProvider.Url, emProvider.Port, emProvider.SSL);
                client.Authenticate(
                    "vegankoapp@gmail.com",
                    "lOh49YRwZDO85oQyfHfo");

                await client.SendAsync(mimeMsg);
                Console.WriteLine("THe mail has been sent successfully !");

                await client.DisconnectAsync(true);
            }
        }

        private struct EmailProvider
        {
            public EmailProvider(string name, string url, int port, bool ssl = false)
            {
                Name = name;
                Url = url;
                Port = port;
                SSL = ssl;
            }

            public string Name { get; set; }
            public string Url { get; set; }
            public int Port { get; set; }
            public bool SSL { get; set; }
        }
    }
}
