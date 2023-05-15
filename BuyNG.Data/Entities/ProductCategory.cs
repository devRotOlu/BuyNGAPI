namespace BuyNG.Data.Entities
{
    public class ProductCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual IEnumerable<Product> Products { get; set; }
    }
}
