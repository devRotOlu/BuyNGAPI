using System.ComponentModel.DataAnnotations;

namespace BuyNG.Core.EntityDTOs
{
    public class ProductCategoryDTO:CreateProductCategoryDTO
    {
        public int Id { get; set; }

        public IEnumerable<ProductDTO> Products { get; set; }
    }

    public class CreateProductCategoryDTO
    {
        [Required]
        [StringLength(100,MinimumLength =1)]
        public string Name { get; set; }
    } 

    public class UpdateProductCategoryDTO:ProductCategoryDTO
    {

    }
}
