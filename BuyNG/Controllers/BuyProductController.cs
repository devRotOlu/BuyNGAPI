using AutoMapper;
using BuyNG.Core.EntityDTOs;
using BuyNG.Core.IRepository;
using BuyNG.Core.Services;
using BuyNG.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyNG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;

        public CartProductController(IUnitOfWork unitOfWork, IMapper mapper, IAuthManager authManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authManager = authManager;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCartProducts()
        {
            var id = _authManager.User.Id;

            var products = await _unitOfWork.ProductsToBuy.GetAll(product => product.ApplicationUserId == id, new List<string> { "Product.ImageUrls" });

            var productDTO = _mapper.Map<List<ProductToBuyDTO>>(products);

            return Ok(productDTO);
        }

        [Authorize]
        [HttpPost("{id:int}/{quantity:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CartProduct(int id, int quantity)
        {
            if (id < 0 || quantity < 1)
            {
                return BadRequest();
            }

            var similarProduct = await _unitOfWork.ProductsToBuy.GetAll(product => product.ProductId == id && product.ApplicationUserId == _authManager.User.Id,null);

            if (similarProduct != null)
            {
                return BadRequest("Product already added to cart");
            }

            var product = await _unitOfWork.Products.Get(product => product.Id == id, new List<string> { "ImageUrls" });

            if (product == null)
            {
                return BadRequest();
            }

            var productToBuy = new ProductToBuy
            {
                ProductId = id,
                ApplicationUserId = _authManager.User.Id,
                ProductCount = quantity,
            };

            await _unitOfWork.ProductsToBuy.Insert(productToBuy);

            await _unitOfWork.Save();

            return Ok();
        }

        [Authorize]
        [HttpPut("{id:int}/{quantity:int}", Name = "UpdateCartProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCartProduct(int id, int quantity)
        {
            if (id < 0 || quantity < 1)
            {
                return BadRequest();
            }

            var similarProduct = await _unitOfWork.ProductsToBuy.Get(product => product.Id == id && product.ApplicationUserId == _authManager.User.Id);

            if (similarProduct == null)
            {
                return BadRequest();
            }

            var updatedProduct = new ProductToBuy
            {
                Id = id,
                ProductId = similarProduct.ProductId,
                ApplicationUserId = _authManager.User.Id,
                ProductCount = quantity,
            };

            _unitOfWork.ProductsToBuy.Update(updatedProduct);

            await _unitOfWork.Save();

            return Ok();

        }

    }
}
