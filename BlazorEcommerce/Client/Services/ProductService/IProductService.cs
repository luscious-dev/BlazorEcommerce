﻿using BlazorEcommerce.Shared;

namespace BlazorEcommerce.Client.Services.ProductService
{
	public interface IProductService
	{
		event Action ProductsChanged;
		List<Product> Products { get; set; }
		string Message { get; set; }
		int CurrentPage { get; set; }
		int PageCount { get; set; }
		string LastSearchText { get; set; }
		Task GetProducts(string? categoryUrl = null);
        Task<ServiceResponse<Product>> GetProduct(int id);
		Task SearchProducts(string searchText, int page);
		Task GetFeaturedProducts();
		Task<List<string>> GetProductSearchSuggestions(string searchText);
	}
}
