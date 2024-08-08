using BlazorEcommerce.Server.Services.CartService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorEcommerce.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly ICartService cartService;

		public CartController(ICartService cartService)
		{
			this.cartService = cartService;
		}

		[HttpPost("products")]
		public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetCartProduct([FromBody] List<CartItem> items)
		{
			var result = await cartService.GetCartProducts(items);
			return Ok(result);
		}

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> StoreCartItems([FromBody] List<CartItem> items)
        {
            var result = await cartService.StoreCartItems(items);
            return Ok(result);
        }

		[HttpGet("count")]
		public async Task<ActionResult<ServiceResponse<int>>> GetCartItemsCount()
		{
			return await cartService.GetCartItemsCount();
		}

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetDbCartProducts()
        {
            var result = await cartService.GetDbCartProducts();
			return Ok(result);
        }
    }
}
