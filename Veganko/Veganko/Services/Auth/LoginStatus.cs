using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Auth
{
    public enum LoginStatus
    {
        UnknownError,
        Unreachable,
        InvalidCredentials,
        UnconfirmedEmail,
        Success,
    }
}
