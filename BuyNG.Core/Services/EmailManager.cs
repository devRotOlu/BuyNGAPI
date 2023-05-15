using BuyNG.Core.Models;
using BuyNG.Core.Services;
using BuyNG.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BuyNG.Services
{
    public class EmailManager : IEmailManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public EmailManager(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task SendEmailConfirmationEmail(ApplicationUser user, string token, string host, string emailConfirmationPage)
        {
            string? appName = _configuration["AppName"];
            string confirmationLink = emailConfirmationPage;
            string userId = "?id={0}&token={1}";

            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>()
                {
                    user.Email
                },

                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.FirstName),
                    new KeyValuePair<string, string>("{{Link}}",string.Format(host + confirmationLink + userId,user.Id,token)),
                    new KeyValuePair<string, string>("{{AppName}}",appName!),
                    new KeyValuePair<string, string>("{{Token}}",token),
                    new KeyValuePair<string, string>("{{UserId}}",user.Id)
                }
            };

            var isEmailConfirmPage = !string.IsNullOrEmpty(emailConfirmationPage);

            await _emailService.SendEmailForEmailConfirmation(options, isEmailConfirmPage);
        }

        public async Task SendForgotPasswordEmail(ApplicationUser user, string token, string host, string passwordRecoveryPage)
        {
            string confirmationLink = passwordRecoveryPage;
            string userId = "?id={0}&token={1}";

            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>()
                {
                    user.Email
                },

                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.FirstName),
                    new KeyValuePair<string, string>("{{Link}}",string.Format(host + confirmationLink + userId,user.Id,token)),
                    new KeyValuePair<string, string>("{{Token}}",token),
                    new KeyValuePair<string, string>("{{UserID}}",user.Id),
                }
            };

            var isResetPasswordPage = !string.IsNullOrEmpty(passwordRecoveryPage);

            await _emailService.SendEmailForPasswordReset(options, isResetPasswordPage);
        }
    }
}
