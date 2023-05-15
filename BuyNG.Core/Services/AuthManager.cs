using BuyNG.Core.EntityDTOs;
using BuyNG.Core.Models;
using BuyNG.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BuyNG.Core.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private static ApplicationUser _user;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailManager _emailManager;

        public ApplicationUser User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        public AuthManager(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailManager emailManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _emailManager = emailManager;
        }

        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigningCredentials();

            var claims = await GetClaims();

            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var tokenLifetime = Convert.ToDouble(jwtSettings.GetSection("Lifetime").Value);

            var tokenExpirationTime = DateTime.Now.AddMinutes(tokenLifetime);

            var token = new JwtSecurityToken(
                    issuer: jwtSettings.GetSection("Issuer").Value,
                    audience: jwtSettings.GetSection("Issuer").Value,
                    claims: claims,
                    expires: tokenExpirationTime,
                    signingCredentials: signingCredentials
                );

            return token;
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,AuthManager._user.UserName),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Issuer"]!),
            };

            var roles = await _userManager.GetRolesAsync(AuthManager._user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Environment.GetEnvironmentVariable("BuyNGKey");

            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<SignInResult> SignInUser(LoginUserDTO userDTO)
        {
            AuthManager._user = await _userManager.FindByEmailAsync(userDTO.Email);

            return await _signInManager.PasswordSignInAsync(userDTO.Email, userDTO.Password, false, false);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            AuthManager._user = user;

            return user;
        }

        public async Task GenerateEmailConfirmationTokenAsync(ApplicationUser user, string host, string emailConfirmationPage)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = Base64UrlEncoder.Encode(token);

            if (!string.IsNullOrEmpty(token))
            {
                await _emailManager.SendEmailConfirmationEmail(user, encodedToken, host, emailConfirmationPage);
            }
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string uid, string token)
        {
            var user = await _userManager.FindByIdAsync(uid);
            var decodedToken = Base64UrlEncoder.Decode(token);
            return await _userManager.ConfirmEmailAsync(user, decodedToken);
        }

        public async Task GenerateForgotPasswordTokenAsync(ApplicationUser user, string host, string? passwordRecoveryPage)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = Base64UrlEncoder.Encode(token);

            if (!string.IsNullOrEmpty(token))
            {
                await _emailManager.SendForgotPasswordEmail(user, encodedToken, host, passwordRecoveryPage);
            }
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model)
        {
            var decodedToken = Base64UrlEncoder.Decode(model.Token);
            return await _userManager.ResetPasswordAsync(await _userManager.FindByIdAsync(model.UserId), decodedToken, model.NewPassword);
        }


        public async Task<IdentityResult> CreateAsync(ApplicationUser appUser, string password)
        {
            return await _userManager.CreateAsync(appUser, password);
        }

        public async Task<IdentityResult> AddToRolesAsync(ApplicationUser appUser, IEnumerable<string> roles)
        {
            return await _userManager.AddToRolesAsync(appUser, roles);
        }
    }
}
