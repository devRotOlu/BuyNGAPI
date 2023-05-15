using BuyNG.Core.Models;

namespace BuyNG.Core.Services
{
    public interface IEmailService
    {
        Task SendTestEmail(UserEmailOptions userEmailOptions);
        Task SendEmailForEmailConfirmation(UserEmailOptions userEmailOptions, bool isEmailConfirmPage);
        Task SendEmailForPasswordReset(UserEmailOptions userEmailOptions, bool isPasswordPage);
    }
}
