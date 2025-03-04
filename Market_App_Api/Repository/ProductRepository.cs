using MarkerAPI.Data;
using MarkerAPI.DTO.Category;
using MarkerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Market_App_Api.Repository;

public interface IProductRepository
{
    Task<Product> GetProductByIdAsync(Guid productId);
    Task<CategorySummary>? GetCategoryByName(string category);
    Task<CategorySummary> CreateCategory(string categoryName);
}

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetProductByIdAsync(Guid productId)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<CategorySummary?> GetCategoryByName(string category)
    {
        return await _context.Categories
            .Where(c => c.Name == category)
            .Select(c => new CategorySummary
            {
                Id = c.Id,
                Name = c.Name
            })
            .FirstOrDefaultAsync();
        
    }

    public async Task<CategorySummary?> CreateCategory(string categoryName)
    {
        var newCategory = new Category
        {
            Name = categoryName
        };

        _context.Categories.Add(newCategory);
        await _context.SaveChangesAsync();

        return await _context.Categories
            .Where(c => c.Name == categoryName)
            .Select(c => new CategorySummary
            {
                Id = c.Id,
                Name = c.Name
            })
            .FirstOrDefaultAsync();
    }

}
