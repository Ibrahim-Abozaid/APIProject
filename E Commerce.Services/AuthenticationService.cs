using E_Commerce.Domain.Entities.IdentityModule;
using E_Commerce.services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<bool> CheckEmailAsync(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            return user != null;
        }

        public async Task<Result<UserDTO>> GetUserByEmailAsync(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
                return Error.NotFound("User.notFound", $"No User With Email {Email} Was Found");

            return new UserDTO(user.Email!, user.DisplayName, await CreateTokenAsync(user));
        }

        public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return Error.InvalidCredentials("User.invalidCredential");
            var IsPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!IsPasswordValid)
                return Error.InvalidCredentials("User.invalidCredential");


            var Token = await CreateTokenAsync(user);

            return new UserDTO(user.Email!, user.DisplayName, Token);

        }

        public async Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO)
        {
            var User = new ApplicationUser
            {
                Email = registerDTO.Email,
                DisplayName = registerDTO.DisplayName,
                PhoneNumber = registerDTO.PhoneNumber,
                UserName = registerDTO.UserName
            };

            var IdentityResult = await _userManager.CreateAsync(User, registerDTO.Password);

            if (IdentityResult.Succeeded)
            {
                var Token = await CreateTokenAsync(User);
                return new UserDTO(User.Email, User.DisplayName, Token);
            }

            return IdentityResult.Errors.Select(E => Error.Validation(E.Code, E.Description)).ToList();
        }


        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!)
            };
            var Roles = await _userManager.GetRolesAsync(user);
            foreach (var role in Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));


            var SecurityKey = _configuration["JWTOptions:SecurityKey"];
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
            var Cred = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var Token = new JwtSecurityToken(
                issuer: _configuration["JWTOptions:Issuer"],
                audience: _configuration["JWTOptions:Audience"],
                expires: DateTime.UtcNow.AddHours(1),
                claims: claims,
                signingCredentials: Cred
                );


            return new JwtSecurityTokenHandler().WriteToken(Token);


        }
    }
}
