using System.ComponentModel.DataAnnotations;

namespace BuyNG.Core.Models
{
    public class RecoverPassword
    {
        [Required]
        public string Email { get; set; }

        public string PasswordRecoveryPage { get; set; }
    }
}
