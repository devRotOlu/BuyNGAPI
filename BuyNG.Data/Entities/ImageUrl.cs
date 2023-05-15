using System.ComponentModel.DataAnnotations.Schema;

//#nullable enable

namespace BuyNG.Data.Entities
{
    public class ImageUrl
    {
        public int Id { get; set; }

        public string Url { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public Product Product { get; set; } 
    }
}
