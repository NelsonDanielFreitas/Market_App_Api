using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkerAPI.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; }

    //public Guid ProductId { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Barcode { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    public Guid CategoryId { get; set; }
    
    [ForeignKey("CategoryId")]
    public Category Category { get; set; }
    
    public string? Brand { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
}