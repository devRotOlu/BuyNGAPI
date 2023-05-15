using AutoMapper;
using BuyNG.Core.EntityDTOs;
using BuyNG.Data.Entities;

namespace BuyNG.Core.Configuration
{
    public class MapperIntializer : Profile
    {
        public MapperIntializer()
        {
            CreateMap<ProductCategory, ProductCategoryDTO>().ReverseMap();
            CreateMap<ProductCategory, CreateProductCategoryDTO>().ReverseMap();
            CreateMap<ProductCategory, UpdateProductCategoryDTO>().ReverseMap();
            CreateMap<ApplicationUser, AppUserDTO>().ReverseMap();
            CreateMap<ApplicationUser, CreateAppUserDTO>().ReverseMap();
            CreateMap<ApplicationUser, UpdateAppUserDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, CreateProductDTO>().ReverseMap();
            CreateMap<Product, UpdateProductDTO>().ReverseMap();
            CreateMap<ImageUrl, ImageUrlDTO>().ReverseMap();
            CreateMap<ProductToBuy, ProductToBuyDTO>().ReverseMap();
            CreateMap<ApplicationUser, AppUserDTO>();
        }
    }
}
