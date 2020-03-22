namespace VegankoService.Helpers
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Rol = "rol", Id = "id";
            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
            }

            public static class Roles
            {
                public const string Admin = "Admin";
                public const string Manager = "Manager";
                public const string Moderator = "Moderator";
                public const string Member = "Member";

                public const string RestrictedAccessRoles = Constants.Strings.Roles.Admin + ", " + Constants.Strings.Roles.Manager + ", " + Constants.Strings.Roles.Moderator;

                public static bool IsInsideRestrictedAccessRoles(string role)
                {
                    return RestrictedAccessRoles.Contains(role);
                }
            }
        }
    }
}
