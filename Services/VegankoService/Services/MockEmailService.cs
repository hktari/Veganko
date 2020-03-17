using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Services
{
    public class MockEmailService : IEmailService
    {
        public static string LastSentToEmail { get; private set; }

        public bool IsEmailProviderSupported(string email) => true;

        public Task SendEmail(string email, string subject, string message)
        {
            LastSentToEmail = email;
            return Task.CompletedTask;
        }
    }
}
