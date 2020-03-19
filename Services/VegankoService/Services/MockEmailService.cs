using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace VegankoService.Services
{
    public class MockEmailService : IEmailService
    {
        public static string LastSentToEmail { get; private set; }

        public bool IsEmailProviderSupported(string email) => true;

        public Task SendEmail(string email, string subject, string message)
        {
            return SendEmail(new MailAddress(email), subject, message); 
        }

        public Task SendEmail(MailAddress email, string subject, string message)
        {
            LastSentToEmail = email.Address;
            return Task.CompletedTask;
        }
    }
}
