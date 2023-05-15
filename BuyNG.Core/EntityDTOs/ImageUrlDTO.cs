using System.ComponentModel.DataAnnotations;

namespace BuyNG.Core.EntityDTOs
{
    public class ImageUrlDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
