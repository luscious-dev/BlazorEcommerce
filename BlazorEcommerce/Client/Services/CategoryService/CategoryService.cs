using BlazorEcommerce.Shared;
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<Category> Categories { get; set; } = new List<Category>();

        public async Task GetCategories()
        {
            var res = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Category>>>("/api/category");

            if(res != null && res.Data != null)
            {
                Categories = res.Data;
            }
        }
    }
}
