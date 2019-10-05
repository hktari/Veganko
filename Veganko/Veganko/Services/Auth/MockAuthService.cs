using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Veganko.Services.Auth
{
    public class MockAuthService : IAuthService
    {
        public Task<bool> CredentialsExist()
        {
            return Task.FromResult(false);
        }

        public Task<string> GetToken()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTokenValid()
        {
            return Task.FromResult(false);
        }

        public Task Login(string email, string password)
        {
            return Task.CompletedTask;
        }

        public void Logout()
        {
        }

        public Task RefreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
