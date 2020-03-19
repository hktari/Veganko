using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Veganko.Services.Auth
{
    public interface IAuthService
    {
        public enum LoginStatus 
        {
            UnknownError,
            Unreachable,
            InvalidCredentials,
            UnconfirmedEmail,
            Success,
        }

        Task<string> GetToken();
        Task RefreshToken();
        Task<bool> IsTokenValid();
        //Task Login(string email, string password);
        Task<bool> CredentialsExist();
        void Logout();
        Task<LoginStatus> Login(string email, string password);
    }
}
