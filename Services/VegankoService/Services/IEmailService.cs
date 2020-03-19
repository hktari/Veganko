using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace VegankoService.Services
{
    public interface IEmailService
    {
        bool IsEmailProviderSupported(string email);
        Task SendEmail(string email, string subject, string message);
        Task SendEmail(MailAddress email, string subject, string message);
    }
}
