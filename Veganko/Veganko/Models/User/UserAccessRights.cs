using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Models.User
{
    public enum UserAccessRights
    {
        None = 0,

        ProductsRead = 1,
        ProductsWrite = 2,
        ProductsDelete = 4,

        All = int.MaxValue,
    }
}
