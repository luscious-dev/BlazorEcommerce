
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

        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
        {
            var products = await FindProductsBySearchText(searchText);

			var data = new List<string>();

			foreach(var product in products)
			{
				if(product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
				{
					data.Add(product.Title);
				}

				if(product.Description != null)
				{
					var punctuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();
					var words = product.Description.Split()
						.Select(s => s.Trim(punctuation));

					foreach(var word in words)
					{
						if(word.Contains(searchText, StringComparison.OrdinalIgnoreCase) && !data.Contains(word))
						{
							data.Add(word);
						}
					}
				}
			}

            return new ServiceResponse<List<string>> { Data = data };
        }

        public async Task<ServiceResponse<List<Product>>> SearchProducts(string searchText)
        {
            var data = await FindProductsBySearchText(searchText);

			return new ServiceResponse<List<Product>> { Data = data };
        }

		private async Task<List<Product>> FindProductsBySearchText(string searchText)
		{
            var data = await _context.Products.Where(x => x.Title.ToLower().Contains(searchText.ToLower()) || x.Description.ToLower().Contains(searchText.ToLower()))
                .Include(x => x.Variants)
                .ToListAsync();

			return data;
        }
    }
}
