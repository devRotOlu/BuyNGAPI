using BuyNG.Data.Entities;

namespace BuyNG.Core.EntityDTOs
{
    public class ProductToBuyDTO
    {
        public int Id { get; set; }

        public Product Product { get; set; }

        public int ProductCount { get; set; }
    }
}
