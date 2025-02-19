using MarkerAPI.Data;
using MarkerAPI.DTO.Category;
using MarkerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MarkerAPI.Services;

public class CategoryService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public CategoryService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _config = configuration;
    }

    public async Task<bool> AddCategoryAsync(AddCategoryDTO categoryDto)
    {
        if (await _context.Categories.AnyAsync(u => u.Name == categoryDto.Name))
        {
            return false;
        }

        var category = new Category()
        {
            Name = categoryDto.Name
        };
        
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<List<CategorySummary>> GetAllCategoriesAsync()
    {
        try
        {
            return await _context.Categories
                .Select(c => new CategorySummary
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao buscar categorias", ex);
        }
    }
}