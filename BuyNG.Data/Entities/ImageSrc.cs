using System.ComponentModel.DataAnnotations.Schema;

namespace BuyNG.Data.Entities
{
    public class ImageSrc
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public Product Product { get; set; }
    }
}
