using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tangy_Business.Repository.IRepository;
using Tangy_Models;

namespace TangyWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository ProductRepository)
        {
            _productRepository = ProductRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productRepository.GetAll());
        }
        [HttpGet("GetId")]
        public async Task<IActionResult> GetId(int? productId)
        {
            if(productId == null || productId == 0)
            {
                return BadRequest(new ErrorModelDTO()
                {
                    Title = "",
                    ErrorMessage = "Invalid Id",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var product = await _productRepository.Get(productId.Value);
            if(product == null)
            {
                return BadRequest(new ErrorModelDTO()
                {
                    Title = "",
                    ErrorMessage = "Invalid Id",
                    StatusCode = StatusCodes.Status404NotFound
                }) ;
            }

            return Ok(product);
        }
    }
}
