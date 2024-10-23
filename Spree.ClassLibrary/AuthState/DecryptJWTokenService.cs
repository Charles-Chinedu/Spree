using Spree.Library.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Spree.Library.AuthState
{
    public static class DecryptJWTokenService
    {
        public static CustomUserClaims DecryptToken(string jwToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwToken))
                    return new CustomUserClaims();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwToken);

                var name = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Name);
                var email = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email);
                var role = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Role);
                return new CustomUserClaims(name!.Value, email!.Value, role!.Value);
            }
            catch
            {
                return null!;
            }
        }
    }
}
