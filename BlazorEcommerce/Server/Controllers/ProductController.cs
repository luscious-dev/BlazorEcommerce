using BlazorEcommerce.Server.Data;
using BlazorEcommerce.Server.Services.ProductService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductController(IProductService productService)
		{
			_productService = productService;
		}

		[HttpGet]
		public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts()
		{
			var res = await _productService.GetProductsAsync();
			return Ok(res);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int id)
		{
			var res = await _productService.GetProductAsync(id);
			return Ok(res);
		}

        [HttpGet("category/{categoryUrl}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductsByCategory(string categoryUrl)
        {
            var res = await _productService.GetProductsByCategory(categoryUrl);
            return Ok(res);
        }

        [HttpGet("search/{searchText}/{page}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> SearchProducts(string searchText, int page = 1)
        {
            var res = await _productService.SearchProducts(searchText, page);
            return Ok(res);
        }

        [HttpGet("searchsuggestions/{searchText}")]
        public async Task<ActionResult<ServiceResponse<List<string>>>> GetProductSearchSuggestions(string searchText)
        {
            var res = await _productService.GetProductSearchSuggestions(searchText);
            return Ok(res);
        }

		[HttpGet("featured")]
		public async Task<ActionResult<ServiceResponse<List<Product>>>> GetFeaturedProducts()
		{
			var res = await _productService.GetFeaturedProducts();
			return Ok(res);
		}
	}
}
