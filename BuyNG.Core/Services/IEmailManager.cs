using BuyNG.Data.Entities;

namespace BuyNG.Core.Services
{
    public interface IEmailManager
    {
        Task SendEmailConfirmationEmail(ApplicationUser user, string token, string host, string emailConfirmationPage);
        Task SendForgotPasswordEmail(ApplicationUser user, string token, string host, string passwordRecoveryPage);
    }
}