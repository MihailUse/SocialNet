using System.Security.Claims;

namespace Common.Extentions
{
    public static class ClaimPrincipalExtention
    {
        public static T? GetClaimValue<T>(this ClaimsPrincipal user, string claim)
        {
            string? value = user.Claims.FirstOrDefault(x => x.Type == claim)?.Value;

            if (value == null)
                return default;

            return Utils.Convert<T>(value);
        }
    }
}
