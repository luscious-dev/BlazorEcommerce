using BlazorEcommerce.Shared;
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.ProductService
{
	public class ProductService : IProductService
	{
		public List<Product> Products { get; set; } = new List<Product>();
		private readonly HttpClient _httpClient;
		

		public ProductService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

        public event Action ProductsChanged;

        public async Task GetProducts(string categoryUrl)
		{
			var result = string.IsNullOrEmpty(categoryUrl) ? 
				await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("/api/product"):
                await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"/api/product/category/{categoryUrl}");

			if(result is not null && result.Data is not null)
			{
				Products = result.Data;
			}

			ProductsChanged.Invoke();
        }

        public async Task<ServiceResponse<Product>> GetProduct(int id)
        {
			var result = await _httpClient.GetFromJsonAsync<ServiceResponse<Product>>($"/api/product/{id}");

			return result;
        }
    }
}
