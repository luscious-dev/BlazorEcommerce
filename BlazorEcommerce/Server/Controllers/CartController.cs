using BlazorEcommerce.Server.Services.CartService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
	}
}
