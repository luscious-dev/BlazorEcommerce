
using BlazorEcommerce.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services.ProductService
{
	public class ProductService : IProductService
	{
		private readonly DataContext _context;

		public ProductService(DataContext context)
		{
			_context = context;
		}

        public async Task<ServiceResponse<Product>> GetProductAsync(int id)
        {
			var response = new ServiceResponse<Product>();
            var product = await _context.Products.Where(x => x.Id == id)
				.Include(x => x.Variants)
				.ThenInclude(x => x.ProductType).FirstOrDefaultAsync();
			if (product == null)
			{
				response.Message = "Product not found";
				response.Success = false;
			}
			else
			{
				response.Data = product;
			}

			return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
		{
			var products = await _context.Products
                .Include(x => x.Variants)
				.ToListAsync();
			var response = new ServiceResponse<List<Product>>
			{
				Data = products
			};

			return response;
		}

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
			var response = new ServiceResponse<List<Product>>
			{
				Data = await _context.Products
					.Where(x => x.Category.Url.ToLower().Equals(categoryUrl.ToLower()))
					.ToListAsync()
			};

			return response;
        }
    }
}
