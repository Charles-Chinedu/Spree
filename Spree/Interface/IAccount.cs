using Spree.Libraries.DTOs;
using static Spree.Libraries.Response.CustomResponses;

namespace Spree.Interface
{
    public interface IAccount
    {
        Task<RegistrationResponse> RegisterAsync(RegisterDTO model);

        Task<LoginResponse> LoginAsync(LoginDTO model);

        LoginResponse RefreshToken(UserSession userSession);
    }
}
