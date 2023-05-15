using BuyNG.Data.Entities;

namespace BuyNG.Core.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Product> Products { get; }

        IGenericRepository<ProductToBuy> ProductsToBuy { get; }

        IGenericRepository<ProductCategory> ProductCategories { get; }

        IGenericRepository<ImageUrl> ImageUrls { get; }

        Task Save();
    }
}
