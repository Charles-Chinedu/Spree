using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Spree.Data;
using Spree.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Spree.Libraries.Response.CustomResponses;
using Spree.Libraries.AuthState;
using Spree.Libraries.DTOs;
using Spree.Libraries.Models;
using Spree.Libraries.Response;

namespace Spree.Services
{

    public class AccountService(StoringData storingData, IConfiguration config) : IAccount
    {
        private readonly StoringData _storingData = storingData;
        private readonly IConfiguration _config = config;

        public async Task<LoginResponse> LoginAsync(LoginDTO model)
        {
            var findUser = await GetUser(model.Email);
            if (findUser == null) 
                return new LoginResponse(false, "User not Found");

            if (!BCrypt.Net.BCrypt.Verify(model.Password, findUser.Password))
                return new LoginResponse(true, "Email/Password not valid");

            string jwToken = GenerateToken(findUser);
            return new LoginResponse(true, "Login Successfully", jwToken);
        }

        public LoginResponse RefreshToken(UserSession userSession)
        {
            CustomUserClaims customUserClaims = DecryptJWTokenService.DecryptToken(userSession.JWToken);
            if (customUserClaims is null) return new LoginResponse(false, "Incorrect token");

            string newToken =
                GenerateToken(new ApplicationUser()
                {
                    Name = customUserClaims.Name,
                    Email = customUserClaims.Email,
                    Role = customUserClaims.Role
                });
            return new LoginResponse(true, "New token", newToken);
        }

        public async Task<RegistrationResponse> RegisterAsync(RegisterDTO model)
        {
            var findUser = await GetUser(model.Email);
            if (findUser is not null) 
                return new RegistrationResponse(false, "User already exist");

            // Create the User
            var user = _storingData.Users.Add(
                new ApplicationUser()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
                }).Entity;

            // Check if Admin Role exists
            var checkIfAdminIsCreated = await _storingData.Users
                .FirstOrDefaultAsync(_ => _.Role!.ToLower().Equals("admin"));

            // If no Admin exists, create one
            if (checkIfAdminIsCreated is null)
            {
                user.Role = "Admin";
                await _storingData.SaveChangesAsync();
            }
            else
            {
                var checkIfUserIsCreated = await _storingData.Users
                    .FirstOrDefaultAsync(_ => _.Role!.ToLower().Equals("user"));
                if (checkIfUserIsCreated is null)
                {
                    user.Role = "User";
                    await _storingData.SaveChangesAsync();
                }
                else
                {
                    user.Role = "User";
                    await _storingData.SaveChangesAsync();
                }
            }
            await _storingData.SaveChangesAsync();


            return new RegistrationResponse(true, "Registered Successfully");
        }

        private string GenerateToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role!)
            };

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"]!,
                audience: config["Jwt:Audience"]!,
                claims: userClaims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private async Task<ApplicationUser?> GetUser(string email) => 
            await _storingData.Users.FirstOrDefaultAsync(_ => _.Email == email);

    }
}
