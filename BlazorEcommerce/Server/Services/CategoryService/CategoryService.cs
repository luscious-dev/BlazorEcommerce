
using BlazorEcommerce.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _context;

        public CategoryService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Category>>> GetCategories()
        {
            var categpries = await _context.Categories.ToListAsync();
            return new ServiceResponse<List<Category>>()
            {
                Data = categpries,
            };
        }

        public Task<ServiceResponse<Category>> GetCategory(int id)
        {
            throw new NotImplementedException();
        }
    }
}
