
using BlazorEcommerce.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services.CartService
{
	public class CartService : ICartService
	{
		private readonly DataContext _coontext;

		public CartService(DataContext coontext)
		{
			_coontext = coontext;
		}

		public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
		{
			var result = new ServiceResponse<List<CartProductResponse>>() { Data = new List<CartProductResponse>() };

			foreach(var item in cartItems)
			{
				var product = await _coontext.Products.Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();	

				if(product == null)
				{
					continue;
				}

				var productVariant = await _coontext.ProductVariants
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
	}
}
