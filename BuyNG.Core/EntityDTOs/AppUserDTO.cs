using System.ComponentModel.DataAnnotations;

namespace BuyNG.Core.EntityDTOs
{
    public class UserDTO : LoginUserDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        public IEnumerable<string> Roles { get; set; }
    }

    public class AppUserDTO:UserDTO
    {   
        public string Id { get; set; }

        public IEnumerable<ProductToBuyDTO> ProductsToBuy { get; set; }

        public IEnumerable<ProductDTO> Products { get; set; }
    }

    public class CreateAppUserDTO:UserDTO
    {
        public string? EmailConfirmationPage { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    public class UpdateAppUserDTO:AppUserDTO
    {

    }
}
