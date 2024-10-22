using Spree.Library.DTOs;
using Spree.Library.Response;
using static Spree.Library.Response.CustomResponses;

namespace Spree.Client.Services
{
    public interface IAccountService
    {
        Task<RegistrationResponse> RegisterAsync(RegisterDTO model);

        Task<LoginResponse> LoginAsync(LoginDTO model);

        Task<LoginResponse> RefreshToken(UserSession  userSession);

    }
}
