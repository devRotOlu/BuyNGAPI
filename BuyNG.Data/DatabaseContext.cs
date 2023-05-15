using BuyNG.Data.Entities;
using BuyNG.Data.EntitiesConfiguration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BuyNG.Data
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ProductCategoryConfig());

            builder.ApplyConfiguration(new RolesConfiguration());
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductToBuy> ProductsToBuy { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<ImageUrl> ImageUrls { get; set; }

        //public DbSet<ImageSrc> ImageSources { get; set; }

    }
}
