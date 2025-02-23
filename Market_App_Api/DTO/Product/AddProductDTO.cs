using System.ComponentModel.DataAnnotations;

namespace MarkerAPI.DTO.Product;

public class AddProductDTO
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Barcode { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    public string? Brand { get; set; }
    public DateTime? ExpiryDate { get; set; }
}