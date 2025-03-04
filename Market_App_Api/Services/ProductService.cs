using MarkerAPI.Data;
using MarkerAPI.DTO.Category;
using MarkerAPI.DTO.Product;
using MarkerAPI.Models;
using Market_App_Api.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace MarkerAPI.Services;

public interface IProductService
{
    Task<Product> CreateProductWithBarcode(string barcode);
}

public class ProductService : IProductService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    private readonly IProductRepository _productRepository;
    private readonly HttpClient _httpClient;
    public ProductService(IConfiguration configuration, AppDbContext context, IProductRepository productRepository, HttpClient httpClient)
    {
        _configuration = configuration;
        _context = context;
        _productRepository = productRepository;
        _httpClient = httpClient;
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

    public async Task<Product> GetProductByBarcodeAsync(string barcode)
    {
        return await _context.Products.Include(c => c.Category).FirstOrDefaultAsync(p => p.Barcode == barcode);

    }


    public async Task<Product> CreateProductWithBarcode(string barcode)
    {
        var response = await _httpClient.GetAsync($"https://world.openfoodfacts.net/api/v2/product/{barcode}");
        CategorySummary categorySummary;
        if (response.IsSuccessStatusCode)
        {
            var productDetails = await response.Content.ReadAsStringAsync();
            var productJson = JObject.Parse(productDetails);

            if (productJson["status"].ToString() == "1")
            {
                var categoryNameByAPI = productJson["product"]["category_properties"]["ciqual_food_name:en"].ToString();
                categorySummary = await _productRepository.GetCategoryByName(categoryNameByAPI);
                
                if (categorySummary == null)
                {
                    categorySummary = await _productRepository.CreateCategory(categoryNameByAPI);
                }
                
                var product = new Product
                {
                    Name = productJson["product"]["product_name"].ToString(),
                    Barcode = barcode,
                    Price = 0,
                    CategoryId = categorySummary.Id,
                    Brand = "Sem empresa",
                    //ExpiryDate = new DateTime()
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product;
                
                //return Ok(new { product_name = productName });
            }
        }

        return new Product();

    }
}