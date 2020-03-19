using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Common.Models.Auth
{
    public enum AuthErrorCode
    {
        Unknown,
        InvalidCredentials,
        UnconfirmedEmail,
        UserProfileNotFound,
    }
}
