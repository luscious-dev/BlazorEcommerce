
using BlazorEcommerce.Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorEcommerce.Server.Services.CartService
{
	public class CartService : ICartService
	{
		private readonly DataContext _context;
		private readonly IHttpContextAccessor _contextAccessor;

        public CartService(DataContext coontext, IHttpContextAccessor contextAccessor)
        {
            _context = coontext;
            _contextAccessor = contextAccessor;
        }

        public async Task<ServiceResponse<int>> GetCartItemsCount()
        {
            var count = await _context.CartItems.Where(x => x.UserId == GetUserId()).CountAsync();
			return new ServiceResponse<int> { Data = count };
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
		{
			var result = new ServiceResponse<List<CartProductResponse>>() { Data = new List<CartProductResponse>() };

			foreach(var item in cartItems)
			{
				var product = await _context.Products.Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();	

				if(product == null)
				{
					continue;
				}

				var productVariant = await _context.ProductVariants
					.Where(x => x.ProductId == item.ProductId && x.ProductTypeId == item.ProductTypeId)
					.Include(x => x.ProductType)
					.FirstOrDefaultAsync();

				if(productVariant == null)
				{
					continue;
				}

				result.Data.Add(new CartProductResponse
				{
					ImageUrl = product.ImageUrl,
					ProductTypeId = productVariant.ProductTypeId,
					ProductId = product.Id,
					Price = productVariant.Price,
					ProductType = productVariant.ProductType.Name,
					Title = product.Title,
					Quantity = item.Quantity
				});
			}

			return result;
		}

        public async Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts()
        {
			return await GetCartProducts(await _context.CartItems.Where(x => x.UserId == GetUserId()).ToListAsync());
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems)
        {
			var userId = GetUserId();
            cartItems.ForEach(cartItem => cartItem.UserId = userId);
			_context.CartItems.AddRange(cartItems);
			await _context.SaveChangesAsync();

			return await GetCartProducts(
				await _context.CartItems.Where(x => x.UserId == userId).ToListAsync()
				);
        }

		private int GetUserId() => int.Parse(_contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
