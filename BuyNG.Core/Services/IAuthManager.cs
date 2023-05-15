using BuyNG.Core.EntityDTOs;
using BuyNG.Core.Models;
using BuyNG.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace BuyNG.Core.Services
{
    public interface IAuthManager
    {
        ApplicationUser User { get; set; }
        Task<SignInResult> SignInUser(LoginUserDTO userDTO);
        Task<string> CreateToken();
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task GenerateEmailConfirmationTokenAsync(ApplicationUser user, string host, string emailConfirmationPage);
        Task<IdentityResult> ConfirmEmailAsync(string uid, string token);
        Task GenerateForgotPasswordTokenAsync(ApplicationUser user, string host, string? passwordRecoveryPage);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model);
        Task<IdentityResult> CreateAsync(ApplicationUser appUser, string password);
        Task<IdentityResult> AddToRolesAsync(ApplicationUser appUser, IEnumerable<string> roles);
        Task<ApplicationUser> GetUserByIdAsync(string id);

    }
}
