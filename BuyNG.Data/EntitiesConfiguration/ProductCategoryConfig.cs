using BuyNG.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuyNG.Data.EntitiesConfiguration
{
    public class ProductCategoryConfig : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.HasData(ListCategories());
        }

        private IEnumerable<ProductCategory> ListCategories()
        {
            return new List<ProductCategory>()
            {
                new ProductCategory
                {
                    Id = 1,
                    Name = "Health & Beauty"
                },
                new ProductCategory
                {
                    Id = 2,
                    Name = "Home & Office"
                },
                new ProductCategory
                {
                    Id = 3,
                    Name = "Phones and Tablets"
                },
                new ProductCategory
                {
                    Id = 4,
                    Name = "Computing"
                },
                new ProductCategory
                {
                    Id = 5,
                    Name = "Electronics"
                },
                 new ProductCategory
                {
                    Id = 6,
                    Name = "Fashion"
                },
                  new ProductCategory
                {
                    Id = 7,
                    Name = "Baby Products"
                },
                 new ProductCategory
                {
                    Id = 8,
                    Name = "Gaming"
                },
                  new ProductCategory
                {
                    Id = 9,
                    Name = "Sporting Goods"
                },
                 new ProductCategory
                {
                    Id = 10,
                    Name = "Automobile"
                },
                  new ProductCategory
                {
                    Id = 11,
                    Name = "Other Categories"
                }
            };
        }
    }
}
