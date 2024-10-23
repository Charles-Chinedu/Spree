using Spree.Libraries.DTOs;
using static Spree.Libraries.Response.CustomResponses;

namespace Spree.Client.Services
{
    public interface IAccountService
    {
        Task<RegistrationResponse> RegisterAsync(RegisterDTO model);

        Task<LoginResponse> LoginAsync(LoginDTO model);

        Task<LoginResponse> RefreshToken(UserSession  userSession);

    }
}
