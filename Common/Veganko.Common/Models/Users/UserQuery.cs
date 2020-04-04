using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Common.Models.Users
{
    public class UserQuery
    {
        public UserQuery()
        {

        }

        public UserQuery(UserRole? role = null, int page = 1, int pageSize = -1, string name = null)
        {
            if (page < 1)
            {
                throw new ArgumentException("Min value is 1", nameof(page));
            }

            Role = role;
            Page = page;
            PageSize = pageSize;
            Name = name;
        }

        public string Name { get; set; }
        public UserRole? Role { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = -1;
    }
}
