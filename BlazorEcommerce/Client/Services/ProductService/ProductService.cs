using BlazorEcommerce.Shared;
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.ProductService
{
	public class ProductService : IProductService
	{
		public List<Product> Products { get; set; } = new List<Product>();
        public string Message { get; set; } = "Loading Products...";
        public int CurrentPage { get; set; } = 1;
        public int PageCount { get; set; } = 0;
        public string LastSearchText { get; set; } = string.Empty;

        private readonly HttpClient _httpClient;
		

		public ProductService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

        public event Action ProductsChanged;

        public async Task GetProducts(string categoryUrl)
		{
			var result = string.IsNullOrEmpty(categoryUrl) ? 
				await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("/api/product/featured"):
                await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"/api/product/category/{categoryUrl}");

			if(result is not null && result.Data is not null)
			{
				Products = result.Data;
			}

            CurrentPage = 1;
            PageCount = 0;

            if(Products.Count == 0)
            {
                Message = "No products foun";
            }

			ProductsChanged.Invoke();
        }

        public async Task<ServiceResponse<Product>> GetProduct(int id)
        {
			var result = await _httpClient.GetFromJsonAsync<ServiceResponse<Product>>($"/api/product/{id}");

			return result;
        }

        public async Task SearchProducts(string searchText, int page)
        {
            LastSearchText = searchText;
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<ProductSearchResult>>($"api/product/search/{searchText}/{page}");

            if(result != null && result.Data is not null)
            {
                Products = result.Data.Products;
                CurrentPage = result.Data.CurrentPage;
                PageCount = result.Data.Pages;
            }
            if (Products.Count == 0) Message = "No products found.";
            ProductsChanged?.Invoke();
        }

        public async Task<List<string>> GetProductSearchSuggestions(string searchText)
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/searchsuggestions/{searchText}");

            return result.Data;
        }

		public async Task GetFeaturedProducts()
		{
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/featured");

            if(result != null && result.Data is not null)
            {
                Products = result.Data;
            }

            if (Products.Count == 0) Message = "No Featured Product";
		}
	}
}
