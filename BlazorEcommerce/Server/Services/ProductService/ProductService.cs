
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

		public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
		{
			var response = new ServiceResponse<List<Product>>();
			var products = await _context.Products.Where(x => x.Featured).Include(p => p.Variants).ToListAsync();

			response.Data = products;
			return response;
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
			List<string> result = new List<string>();

			foreach(var product in products)
			{
				if(product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
				{
					result.Add(product.Title);
				}

				if(product.Description != null)
				{
					var punctautions = product.Description.Where(char.IsPunctuation)
						.Distinct().ToArray();

					var words = product.Description.Split()
						.Select(s => s.Trim(punctautions));

					foreach(var word in words)
					{
						if(word.Contains(searchText, StringComparison.OrdinalIgnoreCase) && !result.Contains(word))
						{
							result.Add(word);
						}
					}
				}
			}
			return new ServiceResponse<List<string>> { Data = result };
        }

        public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText, int page)
        {
			var pageResults = 2;
			var pageCount = Math.Ceiling((double)(await FindProductsBySearchText(searchText)).Count / pageResults);

			var products = await _context.Products.Where(x =>
                    x.Title.ToLower().Contains(searchText.ToLower()) || x.Description.ToLower().Contains(searchText.ToLower())
                ).Include(x => x.Variants).Skip((page - 1) * pageResults).Take(pageResults).ToListAsync();

            var response = new ServiceResponse<ProductSearchResult>
			{
				Data = new ProductSearchResult
				{
					Products = products,
					CurrentPage = page,
					Pages = (int)pageCount
				}
			};

			return response;
        }

		private async Task<List<Product>> FindProductsBySearchText(string searchText)
		{
			return await _context.Products.Where(x =>
					x.Title.ToLower().Contains(searchText.ToLower()) || x.Description.ToLower().Contains(searchText.ToLower())
				).Include(x => x.Variants).ToListAsync();
        }
    }
}
