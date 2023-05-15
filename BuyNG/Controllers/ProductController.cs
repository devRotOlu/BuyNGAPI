using AutoMapper;
using BuyNG.Core.EntityDTOs;
using BuyNG.Core.IRepository;
using BuyNG.Core.Models;
using BuyNG.Core.Services;
using BuyNG.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BuyNG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _coverImageFolder;
        private readonly IAuthManager _authManager;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHost, IAuthManager authManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _coverImageFolder = "ProductImages";
            _authManager = authManager;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProducts([FromQuery] RequestParams requestParams)
        {
            var products = await _unitOfWork.Products.GetAll(requestParams,includes: new List<string> { "ImageUrls" });

            var result = _mapper.Map<List<ProductDTO>>(products);

            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }

            var product = await _unitOfWork.Products.Get(product => product.Id == id, new List<string> { "ImageUrls" });

            if (product == null)
            {
                return BadRequest();
            }

            var result = _mapper.Map<ProductDTO>(product);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(productDTO);

            product.ImageUrls = new List<ImageUrl>();

            var domainName = string.Format($"{Request.Scheme}://{Request.Host.Value}");

            foreach (var imageUrl in CreateFileUrl(productDTO.ImageFiles, domainName))
            {
                product.ImageUrls.Add(new ImageUrl
                {
                    Url = imageUrl,
                });
            }

            product.ApplicationUserId = _authManager.User.Id;

            await _unitOfWork.Products.Insert(product);

            await _unitOfWork.Save();

            return StatusCode(201);
        }

        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDTO productDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fetchedproduct = await _unitOfWork.Products.Get(product => product.Id == productDTO.Id && product.ApplicationUserId == _authManager.User.Id);

            var category = await _unitOfWork.ProductCategories.Get(category => category.Id == productDTO.CategoryId);

            if (fetchedproduct == null || category == null)
                return BadRequest(ModelState);

            var product = _mapper.Map<Product>(productDTO);

            if (productDTO.ImageFiles != null)
            {
                var isModified = await ModifyProductImages(product, productDTO);

                if (!isModified)
                    return BadRequest();
            }

            product.ApplicationUserId = _authManager.User.Id;

            _unitOfWork.Products.Update(product);

            await _unitOfWork.Save();

            return CreatedAtRoute("GetProduct", new { id = productDTO.Id }, product);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id < 1)
                return BadRequest();

            var product = await _unitOfWork.Products.Get(product => product.Id == id && product.ApplicationUserId == _authManager.User.Id, new List<string> { "ImageUrls" });

            if (product == null)
            {
                return BadRequest();
            }

            var imageUrls = new List<ImageUrlDTO>();

            foreach (var imageUrl in product.ImageUrls)
            {
                imageUrls.Add(_mapper.Map<ImageUrlDTO>(imageUrl));
            }

            DeleteProductImages(imageUrls);

            _unitOfWork.Products.Delete(product);

            await _unitOfWork.Save();

            return NoContent();

        }

        private void DeleteProductImages(List<ImageUrlDTO> urlDTOs)
        {
            urlDTOs.Where(urlDTO => System.IO.File.Exists(urlDTO.Url)).ToList().ForEach(urlDTO => System.IO.File.Delete(urlDTO.Url));
        }

        private List<string> CreateFileUrl(List<IFormFile> files, string domainName)
        {
            var fileUrlList = new List<string>();

            foreach (var file in files)
            {
                var fileName = Guid.NewGuid().ToString();

                var fileExtension = Path.GetExtension(file.FileName);

                var filePath = Path.Combine(_coverImageFolder, fileName + fileExtension);

                var fileStream = new FileStream(filePath, FileMode.Create);

                file.CopyTo(fileStream);

                fileStream.Close();

                filePath = Path.Combine(domainName, filePath);

                fileUrlList.Add(filePath);
            }

            return fileUrlList;
        }

        private async Task<bool> ModifyProductImages(Product product, UpdateProductDTO productDTO)
        {
            product.ImageUrls = new List<ImageUrl>();

            var index = 0;

            var imageUrls = new List<ImageUrlDTO>();

            var urls = await _unitOfWork.ImageUrls.GetAll(imageUrl => imageUrl.ProductId == product.Id,null);

            var domainName = string.Format($"{Request.Scheme}://{Request.Host.Value}");

            foreach (var imageUrl in CreateFileUrl(productDTO.ImageFiles,domainName))
            {
                var urlId = productDTO.ImageUrlIds[index];

                var url = urls.Where(url => url.Id == urlId).ToArray();

                if (url[0] == null)
                    return false;

                imageUrls.Add(_mapper.Map<ImageUrlDTO>(url[0]));

                product.ImageUrls.Add(new ImageUrl
                {
                    Id = imageUrls[index].Id,
                    Url = imageUrl,
                });

                index++;
            }

            DeleteProductImages(imageUrls);

            return true;
        }

    }
}
