using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BuyNG.Core.EntityDTOs
{
    public class ProductDTO : ProductDTOMain
    {
        
        public int Id { get; set; }

        public List<ImageUrlDTO> ImageUrls { get; set; }
    }

    public class CreateProductDTO : ProductDTOMain
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public virtual List<IFormFile> ImageFiles { get; set; }
    }

    public class UpdateProductDTO : CreateProductDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public List<int> ImageUrlIds { get; set; }

        public override List<IFormFile> ImageFiles { get; set; }
    }

    public class ProductDTOMain
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public uint Quantity { get; set; }

        [MaxLength(150)]
        [Required]
        public string Description { get; set; }

        [Required]
        [MinLengthAttribute(10)]
        public string Location { get;set; }
    }
}
