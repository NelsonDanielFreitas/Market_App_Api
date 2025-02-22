using MarkerAPI.DTO.Category;

namespace MarkerAPI.DTO.Product;

public class ProductDTO
{
    public int Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public string Barcode { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public CategoryDTO? Category { get; set; } // Nullable to handle null categories
    public string Brand { get; set; }
    public DateTime ExpiryDate { get; set; }
}