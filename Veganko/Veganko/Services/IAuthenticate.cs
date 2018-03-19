using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Veganko.Services
{
    public interface IAuthenticate
    {
        Task<bool> Authenticate();
    }
}
