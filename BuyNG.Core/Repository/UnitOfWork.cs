using BuyNG.Core.IRepository;
using BuyNG.Data;
using BuyNG.Data.Entities;

namespace BuyNG.Core.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _databaseContext;

        public IGenericRepository<Product> Products { get; }
        public IGenericRepository<ProductToBuy> ProductsToBuy { get; }
        public IGenericRepository<ProductCategory> ProductCategories { get; }
        public IGenericRepository<ImageUrl> ImageUrls { get; }

        public UnitOfWork(DatabaseContext databaseContext)
        {
            Products = new GenericRepository<Product>(databaseContext);
            ProductsToBuy = new GenericRepository<ProductToBuy>(databaseContext);
            ProductCategories = new GenericRepository<ProductCategory>(databaseContext);
            ImageUrls = new GenericRepository<ImageUrl>(databaseContext);
            _databaseContext = databaseContext;
        }

        public void Dispose()
        {
            _databaseContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}
