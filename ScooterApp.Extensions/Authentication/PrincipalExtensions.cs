using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ScooterApp.Domain.Auth;

namespace ScooterApp.Extensions.Authentication
{
    public static class PrincipalExtensions
    {
        public static RiderClaim ToRiderClaims(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal?.Claims == null || !claimsPrincipal.Claims.Any<Claim>())
                return (RiderClaim)null;

            List<Claim> list = claimsPrincipal.Claims.ToList<Claim>();
            return new RiderClaim
            {
                AreaId = list.GetClaimValue("AreaId"),
                FirstName = list.GetClaimValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"),
                LastName = list.GetClaimValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"),
                UserId = list.GetClaimValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"),
                Claims = list.ToDictionary<Claim, string, string>((Func<Claim, string>)(x => x.Type), (Func<Claim, string>)(x => x.Value)),
                SignInName = list.GetClaimValue("signInName"),
                EmailAddress = list.GetClaimValue("signInName"),
                Salutation = list.GetClaimValue("extension_Salutation")
            };
        }

        private static string GetClaimValue(this IEnumerable<Claim> claims, string name)
        {
            return claims.SingleOrDefault<Claim>((Func<Claim, bool>)(x => x.Type.Equals(name, StringComparison.InvariantCultureIgnoreCase)))?.Value;
        }
    }
}