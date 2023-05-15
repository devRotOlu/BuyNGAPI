using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyNG.Core.Models
{
    public class EmailConfirm
    {
        [Required]
        public string Email { get; set; }

        public string EmailConfirmationPage { get; set; }
    }
}
