using Microsoft.AspNetCore.Components.Authorization;
using Spree.Libraries.Response;
using System.Security.Claims;

namespace Spree.Libraries.AuthState
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Constants.JWToken))
                    return await Task.FromResult(new AuthenticationState(anonymous));

                var getUserClaims = DecryptJWTokenService.DecryptToken(Constants.JWToken);
                if (getUserClaims == null) return await Task.FromResult(new AuthenticationState(anonymous));

                var claimsPrincipal = SetClaimsPrincipal(getUserClaims);
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(anonymous));
            }

        }

        public void UpdateAuthenticationState(string jwToken)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (!string.IsNullOrEmpty(jwToken))
            {
                Constants.JWToken = jwToken;
                var getUserClaims = DecryptJWTokenService.DecryptToken(jwToken);
                claimsPrincipal = SetClaimsPrincipal(getUserClaims);
            }
            else
            {
                Constants.JWToken = null!;
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public static ClaimsPrincipal SetClaimsPrincipal(CustomUserClaims claims)
        {
            if (claims.Email is null) return new ClaimsPrincipal();
            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.Name, claims.Name),
                    new(ClaimTypes.Email, claims.Email!),
                    new(ClaimTypes.Role, claims.Role!)
                }, "JwtAuth"));
        }


    }
}
