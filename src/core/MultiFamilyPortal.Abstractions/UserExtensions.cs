namespace System.Security.Claims
{
    public static class UserExtensions
    {
        public static bool IsInAnyRole(this ClaimsPrincipal user, params string[] roles)
        {
            foreach (var role in roles)
                if (user.IsInRole(role))
                    return true;

            return false;
        }
    }
}
