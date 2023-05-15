using System.ComponentModel.DataAnnotations.Schema;

namespace BuyNG.Data.Entities
{
    public class ProductToBuy
    {
        public int Id { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int ProductCount { get; set; }
    }
}
