using MarkerAPI.Data;
using MarkerAPI.DTO.Product;
using MarkerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MarkerAPI.Services;

public class ProductService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public ProductService(IConfiguration configuration, AppDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }
    
    public async Task<Product> AddProductAsync(AddProductDTO addProductDto)
    {
        var product = new Product
        {
            Name = addProductDto.Name,
            Barcode = addProductDto.Barcode,
            Price = addProductDto.Price,
            CategoryId = addProductDto.CategoryId,
            Brand = addProductDto.Brand,
            ExpiryDate = addProductDto.ExpiryDate
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> CheckCategorie(AddProductDTO addProductDto)
    {
        return !await _context.Categories.AnyAsync(u => u.Id == addProductDto.CategoryId);
    }

    public async Task<bool> CheckBarcode(string barcode)
    {
        return await _context.Products.AnyAsync(u => u.Barcode == barcode);
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products.Include(c => c.Category).ToListAsync();
    }

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<Product> GetProductByIdAsync(Guid productId)
    {
        return await _context.Products.Include(c => c.Category).FirstOrDefaultAsync(p => p.Id == productId);
    }
}