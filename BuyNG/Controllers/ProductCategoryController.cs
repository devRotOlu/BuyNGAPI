using AutoMapper;
using BuyNG.Core.EntityDTOs;
using BuyNG.Core.IRepository;
using BuyNG.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BuyNG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        public ProductCategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategories()
        {
          
            var categories = await _unitOfWork.ProductCategories.GetAll<Product>(includes: productCat => productCat.Include(productCategory=> productCategory.Products).ThenInclude(product=> product.ImageUrls));

            var result = _mapper.Map<List<ProductCategoryDTO>>(categories);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCategory(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }

            var category = await _unitOfWork.ProductCategories.Get(category => category.Id == id, new List<string> { "Products" });

            if (category == null)
            {
                return BadRequest();
            }

            var result = _mapper.Map<ProductCategoryDTO>(category);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostCategories([FromBody] List<CreateProductCategoryDTO> productCategories)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var categoryList = _mapper.Map<List<ProductCategory>>(productCategories);
            await _unitOfWork.ProductCategories.InsertRange(categoryList);
            await _unitOfWork.Save();

            return Ok();
        }
    }

}
