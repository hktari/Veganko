using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Models.User
{
    public enum UserRole
    {
        Member,
        Moderator,
        Manager,
        Admin
    }

    public static class UserRoleExtensions
    {
        public static UserAccessRights ToUAC(this UserRole userRole)
        {
            UserAccessRights rights;
            switch (userRole)
            {
                case UserRole.Member:
                    rights = UserAccessRights.ProductsRead;
                    break;
                case UserRole.Moderator:
                    rights = UserAccessRights.All;
                    break;
                case UserRole.Manager:
                    rights = UserAccessRights.All;
                    break;
                case UserRole.Admin:
                    rights = UserAccessRights.All;
                    break;
                default:
                    throw new Exception("Trying to convert unhandled user role to user acces rights");
            }

            return rights;
        }
    }
}
