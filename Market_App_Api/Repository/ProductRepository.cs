using MarkerAPI.Data;
using MarkerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Market_App_Api.Repository;

public interface IProductRepository
{
    Task<Product> GetProductByIdAsync(Guid productId);
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
}
