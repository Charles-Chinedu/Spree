using Spree.Library.DTOs;
using Spree.Library.Response;
using static Spree.Library.Response.CustomResponses;

namespace Spree.Interface
{
    public interface IAccount
    {
        Task<RegistrationResponse> RegisterAsync(RegisterDTO model);

        Task<LoginResponse> LoginAsync(LoginDTO model);

        LoginResponse RefreshToken(UserSession userSession);
    }
}
