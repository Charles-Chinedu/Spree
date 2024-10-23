using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spree.Interface;
using Spree.Libraries.DTOs;
using static Spree.Libraries.Response.CustomResponses;

namespace Spree.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IAccount accountService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> RegisterAsync(RegisterDTO model)
        {
            var result = await accountService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public ActionResult<LoginResponse> RefreshToken(UserSession model)
        {
            var result = accountService.RefreshToken(model);
            return Ok(result);
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LoginAsync(LoginDTO model)
        {
            var result = await accountService.LoginAsync(model);
            return Ok(result);
        }
    }
}
